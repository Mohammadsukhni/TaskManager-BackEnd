using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core.Entities;
using TaskManager.Core.Enum;
using WorkItemType = TaskManager.Core.Enum.WorkItemType;

namespace TaskManager.Core.Entities
{
    public class WorkItem : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TimeSpan EstimatedTime { get; set; }
        public TimeSpan ActualTime { get; set; }
        public Status Status { get; set; }
        public WorkItemType Type { get; set; }
        public string ReferenceNumber { get; set; } = string.Empty;

        public int? AssignedToUserId { get; set; }
        public User? AssignedToUser { get; set; }
        public int SprintId { get; set; }
        public Sprint Sprint { get; set; } = null!;
        public ICollection<WorkItemRelation> ParentRelations { get; set; } = new List<WorkItemRelation>();

        public ICollection<WorkItemRelation> ChildRelations { get; set; } = new List<WorkItemRelation>();

    }
}
