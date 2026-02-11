using EduNexus.Core.IRepositories.Generic;
using EduNexus.Core.Models.V1.Dtos.Employee;
using EduNexus.Domain.Entities.Business;

namespace EduNexus.Core.IRepositories.Non_Generic
{
    public interface IEmployeeRepositoryAsync : IGenericRepositoryAsync<Employee>
    {
        Task<EmployeeDto?> GetEmployee(Guid id);
    }
}
