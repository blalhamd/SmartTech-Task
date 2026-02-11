using EduNexus.Core.Constants;
using EduNexus.Core.IServices;
using EduNexus.Core.IUnit;
using EduNexus.Core.Models.V1.Dtos.EmployeeRequest;
using EduNexus.Core.Models.V1.ViewModels.EmployeeRequest;
using EduNexus.Domain.Entities.Business;
using EduNexus.Domain.Entities.Identity;
using EduNexus.Domain.Enums;
using EduNexus.Domain.Errors;
using EduNexus.Shared;
using EduNexus.Shared.Common;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Transactions;
using Error = EduNexus.Shared.Common.Error;

namespace EduNexus.Business.Services
{
    public class EmployeeRequestService : IEmployeeRequestService
    {
        private readonly IUnitOfWorkAsync _uOW;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<EmployeeRequestService> _logger;
        private readonly IValidator<CreateEmployeeRequestDto> _createValidator;
        private readonly IValidator<UpdateEmployeeRequestDto> _updateValidator;
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            WriteIndented = false, 
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        public EmployeeRequestService(IUnitOfWorkAsync uOW,
            UserManager<ApplicationUser> userManager,
            ICurrentUserService currentUserService,
            ILogger<EmployeeRequestService> logger,
            IValidator<CreateEmployeeRequestDto> createValidator,
            IValidator<UpdateEmployeeRequestDto> updateValidator)
        {
            _uOW = uOW;
            _userManager = userManager;
            _currentUserService = currentUserService;
            _logger = logger;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }


        public async Task<ValueResult<Guid>> CreateEmployeeRequestAsync(CreateEmployeeRequestDto dto, CancellationToken cancellation = default)
        {
            _logger.LogInformation("Creating employee request for employee: {FullName}", dto.FullName);

            // validation
            var validationResult = await _createValidator.ValidateAsync(dto, cancellation);
            if (!validationResult.IsValid)
            {
                var error = validationResult.Errors.FirstOrDefault();
                return ValueResult<Guid>.Failure(new Error(error!.ErrorCode, error.ErrorMessage, ErrorType.Validation));
            }

            var isExist = await _userManager.Users.AnyAsync(x => x.Email == dto.Email);
            if (isExist)
            {
                _logger.LogWarning("Attempt to create request for existing email: {Email}", dto.Email);
                return ValueResult<Guid>.Failure(EmployeeErrors.Conflict);
            }

            var serializeData = JsonSerializer.Serialize(dto, _jsonOptions);

            var requestResult = EmployeeRequest.Create(serializeData);
            if (!requestResult.IsSuccess)
                return ValueResult<Guid>.Failure(requestResult.Error);

            await _uOW.EmployeeRequestRepositoryAsync.CreateAsync(requestResult.Value);
            await _uOW.SaveChangesAsync(cancellation);

            return ValueResult<Guid>.Success(requestResult.Value.Id);
        }

        public async Task<Result> ApproveCreateEmployeeRequestAsync(Guid requestId, CancellationToken cancellation = default)
        {
            var employeeRequest = await _uOW.EmployeeRequestRepositoryAsync.GetByIdAsync(requestId);
            if (employeeRequest is null)
                return Result.Failure(EmployeeRequestErrors.NotFound);

            if (employeeRequest.CreatedBy == _currentUserService.UserId)
                return Result.Failure(new Error("Security", "must approved by checker not maker", ErrorType.Validation));

            var approveStatus = employeeRequest.Approve();
            if (!approveStatus.IsSuccess)
                return approveStatus;

            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {

                await _uOW.EmployeeRequestRepositoryAsync.UpdateAsync(employeeRequest);

                var data = JsonSerializer.Deserialize<CreateEmployeeRequestDto>(employeeRequest.NewData, _jsonOptions);
                var user = new ApplicationUser
                {
                    Email = data?.Email,
                    UserName = data?.Email,
                    FullName = data?.FullName!,
                    EmailConfirmed = true
                };

                var identityResult = await _userManager.CreateAsync(user);
                if (!identityResult.Succeeded)
                {
                    var error = identityResult.Errors.FirstOrDefault();
                    return Result.Failure(new Shared.Common.Error(error!.Code, error.Description, ErrorType.Validation));
                }

                var employeeResult = Employee.Create(data!.FullName, data.Salary, data.Position, user!.Id);
                if (!employeeResult.IsSuccess)
                    return Result.Failure(employeeResult.Error);

                await _uOW.EmployeeRepositoryAsync.CreateAsync(employeeResult.Value);
                await _userManager.AddToRoleAsync(user, ApplicationRoles.Employee);

                // send email to employee to welcome and to set password belong him.

                await _uOW.SaveChangesAsync(cancellation);

                transaction.Complete();

                _logger.LogInformation("Employee Created Successfully. EmpId: {EmpId}, ApprovedBy: {CheckerId}",
                                employeeResult.Value.Id, _currentUserService.UserId);

                return Result.Success();
            }
        }

        public async Task<Result> DeleteEmployeeRequestAsync(Guid employeeId, CancellationToken cancellation = default)
        {
            var employee = await _uOW.EmployeeRepositoryAsync.FirstOrDefaultAsync(x => x.Id == employeeId);
            if (employee is null)
                return Result.Failure(EmployeeErrors.NotFound);

            var snapshot = JsonSerializer.Serialize(new
            {
                employee.FullName,
                employee.Position,
                employee.Salary
            }, _jsonOptions);

            var request = EmployeeRequest.CreateDeleteRequest(employeeId, snapshot);
            if(!request.IsSuccess)
                return Result.Failure(request.Error);

            await _uOW.EmployeeRequestRepositoryAsync.CreateAsync(request.Value);
            await _uOW.SaveChangesAsync(cancellation);

            return Result.Success();
        }

        public async Task<Result> ApproveDeleteRequest(Guid requestId, CancellationToken cancellation = default)
        {
            var employeeRequest = await _uOW.EmployeeRequestRepositoryAsync.GetByIdAsync(requestId);
            if (employeeRequest is null)
                return Result.Failure(EmployeeRequestErrors.NotFound);

            var employee = await _uOW.EmployeeRepositoryAsync.FirstOrDefaultAsync(x => x.Id == employeeRequest.EmployeeId);
            if (employee is null)
                return Result.Failure(EmployeeErrors.NotFound);

            await _uOW.EmployeeRepositoryAsync.DeleteAsync(employee);

            var result = employeeRequest.Approve();
            if (!result.IsSuccess)
                return result;

            await _uOW.SaveChangesAsync(cancellation);
            return Result.Success();
        }

        public async Task<ValueResult<PagesResult<EmployeeRequestViewModel>>> GetAllEmployeeRequestsAsync(int pageNumber, int pageSize, CancellationToken cancellation = default)
        {
            pageNumber = Math.Max(pageNumber, 1);
            pageSize = Math.Clamp(pageSize, 1, 10);

            var requests = await _uOW.EmployeeRequestRepositoryAsync
                                .GetAllAsync(expression: x=> x.Status == RequestStatus.Pending, orderBy: o => o.OrderByDescending(x => x.CreatedAt), pageNumber, pageSize);
            if (!requests.Any())
                return ValueResult<PagesResult<EmployeeRequestViewModel>>.Success(new PagesResult<EmployeeRequestViewModel>([], pageNumber, pageSize, 0));

            var totalCount = await _uOW.EmployeeRequestRepositoryAsync.GetCountAsync(x => x.Status == RequestStatus.Pending);

            var deserlizedData = requests.Select(r =>
            {
                var jsonToParse = r.ActionType == ActionType.Delete ? r.OldData : r.NewData;

                CreateEmployeeRequestDto? data = null;

                if (!string.IsNullOrWhiteSpace(jsonToParse))
                {
                    try
                    {
                        data = JsonSerializer.Deserialize<CreateEmployeeRequestDto>(jsonToParse, _jsonOptions);
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Failed to deserialize data for request {Id}", r.Id);
                    }
                }

                return new EmployeeRequestViewModel
                {
                    Id = r.Id,
                    EmployeeId = r.EmployeeId,
                    ActionType = r.ActionType,
                    Status = r.Status,
                    RejectionReason = r.RejectionReason,
                    FullName = data?.FullName,
                    Email = data?.Email,
                    Salary = data?.Salary,
                    Position = data?.Position,
                    OldData = r.OldData,
                };
            }).ToList();

            var pagedResult = new PagesResult<EmployeeRequestViewModel>(deserlizedData, pageNumber, pageSize, totalCount);

            return ValueResult<PagesResult<EmployeeRequestViewModel>>.Success(pagedResult);
        }

        public async Task<ValueResult<EmployeeRequestViewModel>> GetEmployeeRequestByIdAsync(Guid requestId, CancellationToken cancellation = default)
        {
            var employeeRequest = await _uOW.EmployeeRequestRepositoryAsync.GetByIdAsync(requestId);
            if (employeeRequest is null)
                return ValueResult<EmployeeRequestViewModel>.Failure(EmployeeRequestErrors.NotFound);

            var deserilzedData = JsonSerializer.Deserialize<CreateEmployeeRequestDto>(employeeRequest.NewData, _jsonOptions);
            var empRequestVM = new EmployeeRequestViewModel
            {
                ActionType = employeeRequest.ActionType,
                Status = employeeRequest.Status,
                Email = deserilzedData?.Email,
                EmployeeId = employeeRequest.EmployeeId,
                FullName = deserilzedData?.FullName,
                Id = employeeRequest.Id,
                Position = deserilzedData?.Position,
                RejectionReason = employeeRequest?.RejectionReason,
                Salary = deserilzedData?.Salary,
                OldData = employeeRequest?.OldData
            };

            return ValueResult<EmployeeRequestViewModel>.Success(empRequestVM);
        }

        public async Task<Result> RejectEmployeeRequestAsync(Guid requestId, string? Reason, CancellationToken cancellation = default)
        {
            var employeeRequest = await _uOW.EmployeeRequestRepositoryAsync.GetByIdAsync(requestId);
            if (employeeRequest is null)
                return Result.Failure(EmployeeRequestErrors.NotFound);

            var result = employeeRequest.Reject(Reason, _currentUserService.UserId ?? Guid.Empty);
            if (!result.IsSuccess)
                return result;

            await _uOW.EmployeeRequestRepositoryAsync.UpdateAsync(employeeRequest);
            await _uOW.SaveChangesAsync(cancellation);

            _logger.LogInformation("employee request rejected successfully.");

            return Result.Success();
            
        }

        public async Task<ValueResult<Guid>> UpdateEmployeeRequestAsync(Guid employeeId, UpdateEmployeeRequestDto dto, CancellationToken cancellation = default)
        {
            var validationResult = await _updateValidator.ValidateAsync(dto, cancellation);
            if (!validationResult.IsValid)
            {
                var error = validationResult.Errors.FirstOrDefault();
                return ValueResult<Guid>.Failure(new Error(error!.ErrorCode, error.ErrorMessage, ErrorType.Validation));
            }

            var employee = await _uOW.EmployeeRepositoryAsync.FirstOrDefaultAsync(x => x.Id == employeeId);
            if (employee is null)
                return ValueResult<Guid>.Failure(EmployeeErrors.NotFound);

            var oldJson = JsonSerializer.Serialize(new
            {
                employee.FullName,
                employee.Position,
                employee.Salary
            },_jsonOptions);

            var newJson = JsonSerializer.Serialize(new
            {
                dto.FullName,
                dto.Position,
                dto.Salary
            }, _jsonOptions);

            var updateResult = EmployeeRequest.CreateUpdateRequest(employeeId, oldJson, newJson);
            if(!updateResult.IsSuccess)
                return ValueResult<Guid>.Failure(updateResult.Error);

            await _uOW.EmployeeRequestRepositoryAsync.CreateAsync(updateResult.Value);
            await _uOW.SaveChangesAsync(cancellation);

            return ValueResult<Guid>.Success(updateResult.Value.Id);
        }

        public async Task<ValueResult<Guid>> ApproveUpdateEmployeeRequestAsync(Guid requestId, CancellationToken cancellation = default)
        {
            var employeeRequest = await _uOW.EmployeeRequestRepositoryAsync.FirstOrDefaultAsync(x => x.Id == requestId);
            if (employeeRequest is null)
                return ValueResult<Guid>.Failure(EmployeeRequestErrors.NotFound);

            var deserialize = JsonSerializer.Deserialize<CreateEmployeeRequestDto>(employeeRequest.NewData, _jsonOptions);

            var employee = await _uOW.EmployeeRepositoryAsync.FirstOrDefaultAsync(_ => _.Id == employeeRequest.EmployeeId);
            if(employee is null)
                return ValueResult<Guid>.Failure(EmployeeErrors.NotFound);

            var result = employee.Update(deserialize!.FullName, deserialize.Salary, deserialize.Position);
            if(!result.IsSuccess)
                return ValueResult<Guid>.Failure(result.Error);

            var approved = employeeRequest.Approve();
            if(!approved.IsSuccess)
                return ValueResult<Guid>.Failure(approved.Error);

            await _uOW.EmployeeRepositoryAsync.UpdateAsync(employee);
            await _uOW.SaveChangesAsync(cancellation);

            _logger.LogInformation("update request approved successfully");
            return ValueResult<Guid>.Success(requestId);
        }
    }
}
