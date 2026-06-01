using System;
using TaskManager.Core.Dto;
using TaskManager.Core.Entities;
using TaskManager.Core.IRepositories;
using TaskManager.Core.IService;
using TaskManager.Infrastructure.Data;
using TaskManager.Infrastructure.Helper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


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

        public Task Delete(T entity)
        {
            entity.IsDeleted = true;
            entity.LastUpdatedDate = DateTime.Now;
            entity.LastUpdatedById = _currentUserService.GetAuditUserId();

            _context.Set<T>().Update(entity);

            return Task.CompletedTask;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await _context.Set<T>().Where(predicate).ToListAsync();
        }

        public async Task<PagedResultDto<T>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await PaginationHelper.ToPagedResultAsync(
                _context.Set<T>(),
                pageNumber,
                pageSize);
        }

        public async Task<PagedResultDto<T>> GetPagedAsync(
            Expression<Func<T, bool>> predicate,
            int pageNumber,
            int pageSize)
        {
            return await PaginationHelper.ToPagedResultAsync(
                _context.Set<T>().Where(predicate),
                pageNumber,
                pageSize);
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
        }

        public Task Update(T entity)
        {
            entity.LastUpdatedDate = DateTime.Now;
            entity.LastUpdatedById = _currentUserService.GetAuditUserId();

            _context.Set<T>().Update(entity);

            return Task.CompletedTask;
        }
    }
}
