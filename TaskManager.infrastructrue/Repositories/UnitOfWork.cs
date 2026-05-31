using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core.Entities;
using TaskManager.Core.IRepositories;
using TaskManager.Core.IService;
using TaskManager.Infrastructure.Data;

namespace TaskManager.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public UnitOfWork(AppDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;

            Users = new GenericRepository<User>(_context, _currentUserService);
            Projects = new GenericRepository<Project>(_context, _currentUserService);
            UserProjects = new GenericRepository<UserProject>(_context, _currentUserService);
            Sprints = new GenericRepository<Sprint>(_context, _currentUserService);
            WorkItems = new GenericRepository<WorkItem>(_context, _currentUserService);
            WorkItemRelations = new GenericRepository<WorkItemRelation>(_context, _currentUserService);
            Otps = new GenericRepository<Otp>(_context, _currentUserService);
        }

        public IGenericRepository<User> Users { get; }

        public IGenericRepository<Project> Projects { get; }

        public IGenericRepository<UserProject> UserProjects { get; }

        public IGenericRepository<Sprint> Sprints { get; }

        public IGenericRepository<WorkItem> WorkItems { get; }

        public IGenericRepository<WorkItemRelation> WorkItemRelations { get; }

        public IGenericRepository<Otp> Otps { get; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
