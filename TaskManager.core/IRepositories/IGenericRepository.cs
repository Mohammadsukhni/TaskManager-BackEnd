using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core.Entities;

namespace TaskManager.Core.IRepositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task CreateAsync(T entity);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        void Update(T entity);
        void Delete(T entity);
    }
}
