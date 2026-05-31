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
        public int? CreatedById { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public int? LastUpdatedById { get; set; }
    }
}
 

