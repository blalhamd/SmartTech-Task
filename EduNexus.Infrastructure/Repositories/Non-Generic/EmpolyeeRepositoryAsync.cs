using EduNexus.Core.IRepositories.Non_Generic;
using EduNexus.Core.Models.V1.Dtos.Employee;
using EduNexus.Core.Models.V2.Dto;
using EduNexus.Domain.Entities.Business;
using EduNexus.Infrastructure.Data.Context;
using EduNexus.Infrastructure.Repositories.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EduNexus.Infrastructure.Repositories.Non_Generic
{
    public class EmpolyeeRepositoryAsync : GenericRepositoryAsync<Employee>, IEmployeeRepositoryAsync
    {
        private readonly AppDbContext _appDbContext;
        public EmpolyeeRepositoryAsync(AppDbContext context) : base(context)
        {
            _appDbContext = context;
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

     
        public async Task<IList<EmployeeDto>> GetEmployees(Expression<Func<Employee, bool>>? expression, int? pageNumber = null, int? pageSize = null)
        {
            IQueryable<Employee> employees = base.Entity;

            // 2. Apply Filter
            
            if (expression is not null)
            {
                employees = employees.Where(expression).OrderByDescending(x => x.CreatedAt);
            }

            // 4. Apply Pagination

            if (pageNumber.HasValue && pageSize.HasValue)
            {
                var validPageNumber = pageNumber.Value > 0 ? pageNumber.Value : 1;
                var validPageSize = pageSize.Value > 0 ? pageSize.Value : 10;

                employees = employees.Skip((validPageNumber - 1) * validPageSize).Take(validPageSize);
            }

            // Map to dto

            return await employees.Select(x => new EmployeeDto
            {
                Id = x.Id,
                FullName = x.FullName,
                Salary = x.Salary,
                UserId = x.UserId,
                CreatedAt = x.CreatedAt,
                IsActive = x.IsActive,
                Position = x.Position
            })
                .AsNoTracking() // this is the default, because I will return dto not object iteself, so can remove it
                .ToListAsync();
        }

        public async Task<IList<EmployeeDtoV2>> GetEmployeesV2(Expression<Func<Employee, bool>>? expression, int? pageNumber = null, int? pageSize = null)
        {
            IQueryable<Employee> employees = base.Entity;

            // 2. Apply Filter

            if (expression is not null)
            {
                employees = employees.Where(expression).OrderByDescending(x => x.CreatedAt);
            }

            // 4. Apply Pagination

            if (pageNumber.HasValue && pageSize.HasValue)
            {
                var validPageNumber = pageNumber.Value > 0 ? pageNumber.Value : 1;
                var validPageSize = pageSize.Value > 0 ? pageSize.Value : 10;

                employees = employees.Skip((validPageNumber - 1) * validPageSize).Take(validPageSize);
            }

            // Map to dto

            return await employees.Select(x => new EmployeeDtoV2
            {
                Id = x.Id,
                FullName = x.FullName,
                Salary = x.Salary,
                Position = x.Position
            })
                .AsNoTracking()
                .ToListAsync();
        }


        public Task<EmployeeDto?> GetEmployeeV2(Guid id)
        {
            return _getEmployeeById(_appDbContext, id);
        }

       
        private static readonly Func<AppDbContext, Guid, Task<EmployeeDto?>> _getEmployeeById =
            EF.CompileAsyncQuery((AppDbContext context, Guid id) 
                => context.Set<Employee>().Where(x => x.Id == id && x.IsActive)
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
                     .FirstOrDefault());

      
    }
}
