using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core.Entities;

namespace TaskManager.Infrastructure.Data.Configurations
{
    public class UserProjectConfiguration : IEntityTypeConfiguration<UserProject>
    {
        public void Configure(EntityTypeBuilder<UserProject> builder)
        {
            builder.HasKey(up => up.Id);
            // Ensure that a user can only be assigned to a project once
            builder.HasIndex(up => new { up.UserId, up.ProjectId })
                   .IsUnique();
            builder.HasQueryFilter(x => !x.IsDeleted);
            builder.HasOne(up => up.User)
                   .WithMany(u => u.UserProjects)
                   .HasForeignKey(up => up.UserId)
                   .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(up => up.Project)
                .WithMany(p => p.UserProjects)
                .HasForeignKey(up => up.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
