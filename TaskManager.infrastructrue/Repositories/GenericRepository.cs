using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core.Entities;
using TaskManager.Core.IRepositories;
using TaskManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace TaskManager.Infrastructure.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly AppDbContext _context;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(T entity)
        {
            if (string.IsNullOrWhiteSpace(entity.CreatedBy))
                entity.CreatedBy = "System";

            await _context.Set<T>().AddAsync(entity);
        }

        public void Delete(T entity)
        {
            entity.IsDeleted = true;
            entity.LastUpdatedDate = DateTime.Now;
            entity.LastUpdatedBy = "System";

            _context.Set<T>().Update(entity);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
        }

        public void Update(T entity)
        {
            entity.LastUpdatedDate = DateTime.Now;
            entity.LastUpdatedBy = "System";

            _context.Set<T>().Update(entity);
        }
    }
}
