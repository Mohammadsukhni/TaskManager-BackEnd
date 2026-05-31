namespace TaskManager.Core.Dto
{
    public class ProjectFilterResultDto
    {
        public PagedResultDto<ProjectDto> Projects { get; set; } = new();
        public IReadOnlyList<SprintDto> Sprints { get; set; } = new List<SprintDto>();
        public IReadOnlyList<WorkItemDto> WorkItems { get; set; } = new List<WorkItemDto>();
    }
}
