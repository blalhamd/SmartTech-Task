using EduNexus.Core.IRepositories.Generic;
using EduNexus.Core.IRepositories.Non_Generic;
using EduNexus.Domain.Entities.Base;

namespace EduNexus.Core.IUnit
{
    public interface IUnitOfWorkAsync : IAsyncDisposable 
    {
        IGenericRepositoryAsync<T> Repository<T>() where T : BaseEntity;
        IEmployeeRepositoryAsync EmployeeRepositoryAsync { get; }
        IEmployeeRequestRepositoryAsync EmployeeRequestRepositoryAsync { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
