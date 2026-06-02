using TaskManager.Core.Enum;

namespace TaskManager.Core.Dto
{
    public class WorkItemDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public TimeSpan EstimatedTime { get; set; }

        public TimeSpan ActualTime { get; set; }

        public Status Status { get; set; }

        public WorkItemType Type { get; set; }

        public string ReferenceNumber { get; set; } = string.Empty;

        public int? AssignedToUserId { get; set; }

        public int SprintId { get; set; }

        public int? CreatedById { get; set; }

        public int? LastUpdatedById { get; set; }

        public List<WorkItemChildDto> Children { get; set; } = new();
    }

    public class WorkItemChildDto
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public Status Status { get; set; }

        public WorkItemType Type { get; set; }

        public string ReferenceNumber { get; set; } = string.Empty;
    }
}
