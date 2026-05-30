using TaskManager.Core.Dto;
using TaskManager.Core.Entities;

namespace TaskManager.Core.Mapping
{
    public static class WorkItemMapper
    {
        public static WorkItemDto ToDto(this WorkItem workItem)
        {
            ArgumentNullException.ThrowIfNull(workItem);

            return new WorkItemDto
            {
                Id = workItem.Id,
                Title = workItem.Title,
                Description = workItem.Description,
                EstimatedTime = workItem.EstimatedTime,
                ActualTime = workItem.ActualTime,
                Status = workItem.Status,
                Type = workItem.Type,
                ReferenceNumber = workItem.ReferenceNumber,
                AssignedToUserId = workItem.AssignedToUserId,
                SprintId = workItem.SprintId
            };
        }

        public static WorkItem ToEntity(this WorkItemDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);

            return new WorkItem
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
                EstimatedTime = dto.EstimatedTime,
                ActualTime = dto.ActualTime,
                Status = dto.Status,
                Type = dto.Type,
                AssignedToUserId = dto.AssignedToUserId,
                SprintId = dto.SprintId
            };
        }
    }
}
