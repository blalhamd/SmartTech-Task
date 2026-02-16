using EduNexus.Core.IRepositories.Generic;
using EduNexus.Core.Models.V1.Dtos.Employee;
using EduNexus.Core.Models.V2.Dto;
using EduNexus.Domain.Entities.Business;
using System.Linq.Expressions;

namespace EduNexus.Core.IRepositories.Non_Generic
{
    public interface IEmployeeRepositoryAsync : IGenericRepositoryAsync<Employee>
    {
        Task<EmployeeDto?> GetEmployee(Guid id);
        Task<IList<EmployeeDto>> GetEmployees(Expression<Func<Employee, bool>>? expression,
            int? pageNumber = null,
            int? pageSize = null);

        Task<IList<EmployeeDtoV2>> GetEmployeesV2(Expression<Func<Employee, bool>>? expression,
            int? pageNumber = null,
            int? pageSize = null);
    }
}
