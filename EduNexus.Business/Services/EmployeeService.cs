using EduNexus.Core.IServices;
using EduNexus.Core.IUnit;
using EduNexus.Core.Models.V1.Dtos.Employee;
using EduNexus.Core.Models.V1.ViewModels.Employee;
using EduNexus.Domain.Entities.Business;
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
            var employeeDto = await _uOW.EmployeeRepositoryAsync.GetEmployee(id);
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
            pageNumber = Math.Max(pageNumber, 1);
            pageSize = Math.Clamp(pageSize, 1, 10);

            var employees = await _uOW.EmployeeRepositoryAsync
                                 .GetAllAsync(expression: x => x.IsActive,
                                              orderBy: o => o.OrderByDescending(x => x.CreatedAt),
                                              pageNumber, pageSize);
            if (!employees.Any())
                return ValueResult<PagesResult<EmployeeViewModel>>.Success(new([], pageNumber, pageSize, 0));

            var totalCount = await _uOW.EmployeeRepositoryAsync.GetCountAsync(x => x.IsActive);

            var employeesVM = employees.Select(MapEmployeeToEmployeeViewModel).ToList();

            return ValueResult<PagesResult<EmployeeViewModel>>.Success(new(employeesVM, pageNumber, pageSize, totalCount));
        }

        private static EmployeeViewModel MapEmployeeToEmployeeViewModel(Employee employee)
        {
            return new EmployeeViewModel()
            {
                Id = employee.Id,
                FullName = employee.FullName,
                IsActive = employee.IsActive,
                Position = employee.Position,
                Salary = employee.Salary,
                UserId = employee.UserId,
                CreatedAt = employee.CreatedAt,
            };
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
    }
}
