using System.Linq.Expressions;
using TaskManager.Core.Dto;
using TaskManager.Core.Entities;

namespace TaskManager.Core.IRepositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task CreateAsync(T entity);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> predicate);
        Task<PagedResultDto<T>> GetPagedAsync(int pageNumber, int pageSize);
        Task<PagedResultDto<T>> GetPagedAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize);
        Task<T?> GetByIdAsync(int id);
        Task Update(T entity);
        Task Delete(T entity);
    }
}
