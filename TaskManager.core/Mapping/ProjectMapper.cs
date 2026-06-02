using TaskManager.Core.Dto;
using TaskManager.Core.Entities;

namespace TaskManager.Core.Mapping
{
    public static class ProjectMapper
    {
        public static ProjectDto ToDto(this Project project)
        {
            ArgumentNullException.ThrowIfNull(project);

            return new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                ReferenceNumber = project.ReferenceNumber,
                CreatedById = project.CreatedById,
                LastUpdatedById = project.LastUpdatedById
            };
        }

        public static Project ToEntity(this ProjectDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            return new Project
            {
                Id = dto.Id,
                Name = dto.Name
            };
        }
    }
}
