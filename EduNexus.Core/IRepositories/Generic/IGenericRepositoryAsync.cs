using EduNexus.Domain.Entities.Base;
using System.Linq.Expressions;

namespace EduNexus.Core.IRepositories.Generic
{
    public interface IGenericRepositoryAsync<TEntity> where TEntity : BaseEntity
    {
        Task<int> GetCountAsync();
        Task<long> GetCountAsync(Expression<Func<TEntity, bool>> expression);

        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression,
           params Expression<Func<TEntity, object>>[] includes);

        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? expression,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            int? pageNumber = null,
            int? pageSize = null,
            params Expression<Func<TEntity, object>>[] includes);
        Task<TEntity?> GetByIdAsync(Guid id);

        Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> expression);

        Task<TEntity> CreateAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
    }
}
