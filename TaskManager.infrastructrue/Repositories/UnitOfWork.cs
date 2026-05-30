using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core.Entities;
using TaskManager.Core.IRepositories;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;

        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            Users = new GenericRepository<User>(_context);
            Projects = new GenericRepository<Project>(_context);
            UserProjects = new GenericRepository<UserProject>(_context);
            Sprints = new GenericRepository<Sprint>(_context);
            WorkItems = new GenericRepository<WorkItem>(_context);
            WorkItemRelations = new GenericRepository<WorkItemRelation>(_context);
        }

        public IGenericRepository<User> Users { get; }

        public IGenericRepository<Project> Projects { get; }

        public IGenericRepository<UserProject> UserProjects { get; }

        public IGenericRepository<Sprint> Sprints { get; }

        public IGenericRepository<WorkItem> WorkItems { get; }

        public IGenericRepository<WorkItemRelation> WorkItemRelations { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
