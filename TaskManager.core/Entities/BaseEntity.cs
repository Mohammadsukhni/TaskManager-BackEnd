using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Core.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }=false;
        public bool IsActive { get; set; }=true;
        public DateTime CreatedDate { get; set; }=DateTime.Now;
        public string CreatedBy { get; set; } = "System";
        public DateTime? LastUpdatedDate { get; set; }
        public string? LastUpdatedBy { get; set; }
    }
}
 

