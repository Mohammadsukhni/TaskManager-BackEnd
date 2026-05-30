using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core.Dto;

namespace TaskManager.Core.IService
{
    public interface IProjectServices
    {
        Task<IReadOnlyList<ProjectDto>> GetAllProjectsAsync();

        Task<ProjectDto?> GetProjectByIdAsync(int id);

        Task CreateProjectAsync(ProjectDto dto);

        Task UpdateProjectAsync(ProjectDto dto);

        Task DeleteProjectAsync(int id);

        Task AssignUserToProjectAsync(int projectId, int userId);
        Task<IReadOnlyList<ProjectDto>> GetUserProjectsAsync(int userId);
    }
}
