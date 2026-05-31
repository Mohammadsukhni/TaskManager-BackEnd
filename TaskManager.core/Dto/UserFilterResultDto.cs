namespace TaskManager.Core.Dto
{
    public class UserFilterResultDto
    {
        public PagedResultDto<UserDto> Users { get; set; } = new();
        public IReadOnlyList<ProjectDto> Projects { get; set; } = new List<ProjectDto>();
        public IReadOnlyList<SprintDto> Sprints { get; set; } = new List<SprintDto>();
        public IReadOnlyList<WorkItemDto> WorkItems { get; set; } = new List<WorkItemDto>();
    }
}
