using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core.Entities;

namespace TaskManager.Infrastructure.Data.Configurations
{
    public class WorkItemConfiguration : IEntityTypeConfiguration<WorkItem>
    {
        public void Configure(EntityTypeBuilder<WorkItem> builder)
        {
            builder.HasKey(w => w.Id);
            builder.Property(w => w.Id).ValueGeneratedOnAdd();
            builder.Property(w => w.Title)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(w => w.Description)
                .HasMaxLength(500);
            builder.Property(w => w.EstimatedTime).IsRequired();
            builder.Property(w => w.ActualTime).IsRequired();
            builder.Property(w => w.Status).IsRequired();
            builder.Property(w => w.Type).IsRequired();
            builder.Property(w => w.ReferenceNumber)
                .IsRequired();
            builder.HasQueryFilter(w => !w.IsDeleted);
            builder.HasOne(w => w.AssignedToUser)
                 .WithMany(u => u.WorkItems)
                 .HasForeignKey(w => w.AssignedToUserId)
                 .OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(w => w.Sprint)
                 .WithMany(s => s.WorkItems)
                 .HasForeignKey(w => w.SprintId)
                 .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
