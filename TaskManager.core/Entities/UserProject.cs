using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core.Entities;

namespace TaskManager.Core.Entities
{
    public class UserProject: BaseEntity
    {
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public User User { get; set; } = null!;
        public Project Project { get; set; } = null!;
    }
}
