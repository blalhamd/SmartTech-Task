using EduNexus.Core.IRepositories.Generic;
using EduNexus.Core.IRepositories.Non_Generic;
using EduNexus.Core.IUnit;
using EduNexus.Domain.Entities.Base;
using EduNexus.Infrastructure.Data.Context;
using EduNexus.Infrastructure.Repositories.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace EduNexus.Infrastructure.Unit
{
    public class UnitOfWorkAsync : IUnitOfWorkAsync 
    {
        private readonly AppDbContext _context;
        private readonly Dictionary<Type, object> _repositories;
        private readonly IServiceProvider _serviceProvider;

        public UnitOfWorkAsync(AppDbContext context, IServiceProvider serviceProvider)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _repositories = new Dictionary<Type, object>();
            _serviceProvider = serviceProvider;
        }
        public IEmployeeRequestRepositoryAsync EmployeeRequestRepositoryAsync
            => _serviceProvider.GetRequiredService<IEmployeeRequestRepositoryAsync>(); 
        public IEmployeeRepositoryAsync EmployeeRepositoryAsync
            => _serviceProvider.GetRequiredService<IEmployeeRepositoryAsync>();

        public IGenericRepositoryAsync<T> Repository<T>() where T : BaseEntity
        {
            if (_repositories.ContainsKey(typeof(T)))
                return (IGenericRepositoryAsync<T>)_repositories[typeof(T)];

            var repo = new GenericRepositoryAsync<T>(_context);
            _repositories[typeof(T)] = repo;
            return repo;
        }


        public async ValueTask DisposeAsync()
        {
            if(_context is not null)
            {
                await _context.DisposeAsync();
            }
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
