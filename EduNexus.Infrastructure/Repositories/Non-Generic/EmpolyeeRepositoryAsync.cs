using EduNexus.Core.IRepositories.Non_Generic;
using EduNexus.Core.Models.V1.Dtos.Employee;
using EduNexus.Domain.Entities.Business;
using EduNexus.Infrastructure.Data.Context;
using EduNexus.Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;

namespace EduNexus.Infrastructure.Repositories.Non_Generic
{
    public class EmpolyeeRepositoryAsync : GenericRepositoryAsync<Employee>, IEmployeeRepositoryAsync
    {
        public EmpolyeeRepositoryAsync(AppDbContext context) : base(context)
        {
        }

        public async Task<EmployeeDto?> GetEmployee(Guid id)
        {
            var employeeDto = await base.Entity.Where(x => x.Id == id && x.IsActive)
                                               .Select(x => new EmployeeDto
                                               {
                                                   Id = x.Id,
                                                   FullName = x.FullName,
                                                   IsActive = x.IsActive,
                                                   Position = x.Position,
                                                   Salary = x.Salary,
                                                   UserId = x.UserId,
                                                   CreatedAt = x.CreatedAt,
                                               })
                                               .FirstOrDefaultAsync();

            return employeeDto;
        }
    }
}
