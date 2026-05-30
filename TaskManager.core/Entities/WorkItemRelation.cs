using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core.Entities;

namespace TaskManager.Core.Entities
{
    public class WorkItemRelation : BaseEntity
    {
        public int ParentWorkItemId { get; set; }
        public WorkItem ParentWorkItem { get; set; } = null!;

        public int ChildWorkItemId { get; set; }
        public WorkItem ChildWorkItem { get; set; } = null!;
    }
}
