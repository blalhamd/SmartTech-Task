using EduNexus.Core.IServices;
using EduNexus.Core.IUnit;
using EduNexus.Core.Models.V1.Dtos.Employee;
using EduNexus.Core.Models.V1.ViewModels.Employee;
using EduNexus.Core.Models.V2.Dto;
using EduNexus.Core.Models.V2.ViewModels;
using EduNexus.Domain.Errors;
using EduNexus.Shared.Common;
using Microsoft.Extensions.Logging;

namespace EduNexus.Business.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IUnitOfWorkAsync _uOW;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(IUnitOfWorkAsync uOW, ILogger<EmployeeService> logger)
        {
            _uOW = uOW;
            _logger = logger;
        }

        public async Task<ValueResult<EmployeeViewModel>> GetById(Guid id)
        {
            _logger.LogInformation("Attempting to access employee by ID {Id}", id);

            var employeeDto = await _uOW.EmployeeRepositoryAsync.GetEmployeeV2(id);
            if(employeeDto is  null)
            {
                _logger.LogWarning("Employee with ID {Id} not found", id);
                return ValueResult<EmployeeViewModel>.Failure(EmployeeErrors.NotFound);
            }

            var employeeVM = MapDtoToEmployeeViewModel(employeeDto);

            return ValueResult<EmployeeViewModel>.Success(employeeVM);
        }

        public async Task<ValueResult<PagesResult<EmployeeViewModel>>> GetEmployees(int pageNumber, int pageSize)
        {
            _logger.LogInformation("Attempting to access employees in PageNumber {PageNumber} with PageSize {PageSize}", pageNumber, pageSize);

            pageNumber = Math.Max(pageNumber, 1);
            pageSize = Math.Clamp(pageSize, 1, 10);

            var employees = await _uOW.EmployeeRepositoryAsync
                                 .GetEmployees(expression: x => x.IsActive,
                                              pageNumber, pageSize);
            if (!employees.Any())
                return ValueResult<PagesResult<EmployeeViewModel>>.Success(new([], pageNumber, pageSize, 0));

            var totalCount = await _uOW.EmployeeRepositoryAsync.GetCountAsync(x => x.IsActive);

            var employeesVM = employees.Select(MapDtoToEmployeeViewModel).ToList();

            return ValueResult<PagesResult<EmployeeViewModel>>.Success(new(employeesVM, pageNumber, pageSize, totalCount));
        }

        public async Task<ValueResult<PagesResult<EmployeeViewModelV2>>> GetEmployeesV2(int pageNumber, int pageSize)
        {
            pageNumber = Math.Max(pageNumber, 1);
            pageSize = Math.Clamp(pageSize, 1, 10);

            var employees = await _uOW.EmployeeRepositoryAsync
                                 .GetEmployeesV2(expression: x => x.IsActive,
                                              pageNumber, pageSize);
            if (!employees.Any())
                return ValueResult<PagesResult<EmployeeViewModelV2>>.Success(new([], pageNumber, pageSize, 0));

            var totalCount = await _uOW.EmployeeRepositoryAsync.GetCountAsync(x => x.IsActive);

            var employeesVM = employees.Select(MapDtoToEmployeeViewModelV2).ToList();

            return ValueResult<PagesResult<EmployeeViewModelV2>>.Success(new(employeesVM, pageNumber, pageSize, totalCount));
        }

        private static EmployeeViewModel MapDtoToEmployeeViewModel(EmployeeDto dto)
        {
            return new EmployeeViewModel()
            {
                Id = dto.Id,
                FullName = dto.FullName,
                IsActive = dto.IsActive,
                Position = dto.Position,
                Salary = dto.Salary,
                UserId = dto.UserId,
                CreatedAt = dto.CreatedAt,
            };
        }

        private static EmployeeViewModelV2 MapDtoToEmployeeViewModelV2(EmployeeDtoV2 dto)
        {
            return new EmployeeViewModelV2()
            {
                Id = dto.Id,
                FullName = dto.FullName,
                Position = dto.Position,
                Salary = dto.Salary,
            };
        }

    }
}
