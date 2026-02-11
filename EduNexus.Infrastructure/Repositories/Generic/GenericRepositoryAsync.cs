using EduNexus.Core.IRepositories.Generic;
using EduNexus.Domain.Entities.Base;
using EduNexus.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EduNexus.Infrastructure.Repositories.Generic
{
    public class GenericRepositoryAsync<TEntity> : IGenericRepositoryAsync<TEntity>
        where TEntity : BaseEntity
    {

        private readonly AppDbContext _context;

        private readonly DbSet<TEntity> _entity;

        public GenericRepositoryAsync(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _entity = _context.Set<TEntity>();
        }
        protected DbSet<TEntity> Entity => _entity;

        public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? expression,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            int? pageNumber = 1, int? pageSize = 10,
            params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _entity;

            // 1. Apply Includes

            if (includes.Any())
            {
                foreach (var nav in includes)
                {
                    query = query.Include(nav);
                }
            }

            // 2. Apply Filter

            if (expression is not null)
            {
                query = query.Where(expression);
            }

            // 3. Apply Ordering

            if (orderBy is not null)
            {
                query = orderBy(query);
            }

            // 4. Apply Pagination

            if (pageNumber.HasValue && pageSize.HasValue)
            {
                var validPageNumber = pageNumber.Value > 0 ? pageNumber.Value : 1;
                var validPageSize = pageSize.Value > 0 ? pageSize.Value : 10;
                query = query.Skip((validPageNumber - 1) * validPageSize).Take(validPageSize);
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(Guid id)
        {
            return await _entity.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression, params Expression<Func<TEntity, object>>[] includes)
        {
            IQueryable<TEntity> query = _entity.Where(expression);

            if(includes.Any())
            {
                foreach (var nav in includes)
                {
                    query = query.Include(nav);
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await _entity.CountAsync();
        }

        public async Task<long> GetCountAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await _entity.LongCountAsync(expression);
        }

        public async Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> expression)
        {
            return await _entity.AnyAsync(expression);
        }

        public async Task<TEntity> CreateAsync(TEntity entity)
        {
            var entityEntry = await _entity.AddAsync(entity);
            return entityEntry.Entity;
        }

        public Task UpdateAsync(TEntity entity)
        {
            _entity.Update(entity);

            return Task.CompletedTask;
        }

        public Task DeleteAsync(TEntity entity)
        {
            _entity.Remove(entity);
            return Task.CompletedTask;
        }

    }
}
