using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core.Dto;

namespace TaskManager.Core.IService
{
    public interface IProjectServices
    {
        Task<PagedResultDto<ProjectDto>> GetAllProjectsAsync(int pageNumber, int pageSize);
        Task<ProjectFilterResultDto> FilterProjectsAsync(string? search, int pageNumber, int pageSize);

        Task<ProjectDto?> GetProjectByIdAsync(int id);

        Task CreateProjectAsync(ProjectDto dto);

        Task UpdateProjectAsync(ProjectDto dto);

        Task DeleteProjectAsync(int id);

        Task AssignUserToProjectAsync(int projectId, int userId);
        Task<PagedResultDto<ProjectDto>> GetUserProjectsAsync(int userId, int pageNumber, int pageSize);
    }
}
