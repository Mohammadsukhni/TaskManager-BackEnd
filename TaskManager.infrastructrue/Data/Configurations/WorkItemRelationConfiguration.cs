using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core.Entities;

namespace TaskManager.Infrastructure.Data.Configurations
{
    public class WorkItemRelationConfiguration : IEntityTypeConfiguration<WorkItemRelation>
    {
        public void Configure(EntityTypeBuilder<WorkItemRelation> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.HasQueryFilter(x => !x.IsDeleted);
            builder.HasOne(x => x.ParentWorkItem)
                   .WithMany(x => x.ParentRelations)
                   .HasForeignKey(x => x.ParentWorkItemId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.ChildWorkItem)
                   .WithMany(x => x.ChildRelations)
                   .HasForeignKey(x => x.ChildWorkItemId)
                   .OnDelete(DeleteBehavior.NoAction);

        }
    }
}
