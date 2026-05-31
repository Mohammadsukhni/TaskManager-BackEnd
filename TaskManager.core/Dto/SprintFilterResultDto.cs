namespace TaskManager.Core.Dto
{
    public class SprintFilterResultDto
    {
        public PagedResultDto<SprintDto> Sprints { get; set; } = new();
        public IReadOnlyList<WorkItemDto> WorkItems { get; set; } = new List<WorkItemDto>();
    }
}
