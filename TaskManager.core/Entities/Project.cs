using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core.Entities;

namespace TaskManager.Core.Entities
{
    public class Project : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string ReferenceNumber { get; set; } = string.Empty;
        public ICollection<UserProject> UserProjects { get; set; } = new List<UserProject>();
        public ICollection<Sprint> Sprints { get; set; } = new List<Sprint>();
    }
}
