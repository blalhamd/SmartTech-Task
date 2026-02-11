using EduNexus.Core.IRepositories.Non_Generic;
using EduNexus.Domain.Entities.Business;
using EduNexus.Infrastructure.Data.Context;
using EduNexus.Infrastructure.Repositories.Generic;

namespace EduNexus.Infrastructure.Repositories.Non_Generic
{
    public class EmpolyeeRequestRepositoryAsync : GenericRepositoryAsync<EmployeeRequest>, IEmployeeRequestRepositoryAsync
    {
        public EmpolyeeRequestRepositoryAsync(AppDbContext context) : base(context)
        {
        }
    }
}
