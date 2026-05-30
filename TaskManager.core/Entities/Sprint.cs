using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core.Entities;

namespace TaskManager.Core.Entities
{
    public class Sprint : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;
        public ICollection<WorkItem> WorkItems { get; set; } = new List<WorkItem>();
        public string ReferenceNumber { get; set; } = string.Empty;
    }
}
