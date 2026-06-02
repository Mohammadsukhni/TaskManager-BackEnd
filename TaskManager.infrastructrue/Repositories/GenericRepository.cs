using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Dto;
using TaskManager.Core.Entities;
using TaskManager.Core.IRepositories;
using TaskManager.Core.IService;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Helper;

namespace TaskManager.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GenericRepository(AppDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task CreateAsync(T entity)
        {
            entity.CreatedDate = DateTime.Now;
            entity.CreatedById = _currentUserService.GetAuditUserId();

            await _context.Set<T>().AddAsync(entity);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await QueryableSet().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await QueryableSet().Where(predicate).ToListAsync();
        }

        public async Task<PagedResultDto<T>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await PaginationHelper.ToPagedResultAsync(QueryableSet(), pageNumber, pageSize);
        }

        public async Task<PagedResultDto<T>> GetPagedAsync(Expression<Func<T, bool>> predicate, int pageNumber, int pageSize)
        {
            return await PaginationHelper.ToPagedResultAsync(QueryableSet().Where(predicate), pageNumber, pageSize);
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await QueryableSet().FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task Update(T entity)
        {
            entity.LastUpdatedDate = DateTime.Now;
            entity.LastUpdatedById = _currentUserService.GetAuditUserId();

            _context.Set<T>().Update(entity);

            return Task.CompletedTask;
        }

        public Task Delete(T entity)
        {
            entity.IsDeleted = true;
            entity.LastUpdatedDate = DateTime.Now;
            entity.LastUpdatedById = _currentUserService.GetAuditUserId();

            _context.Set<T>().Update(entity);

            return Task.CompletedTask;
        }

        private IQueryable<T> QueryableSet()
        {
            return _context.Set<T>().Where(x => !x.IsDeleted);
        }
    }
}
