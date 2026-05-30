using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core.Entities;
using TaskManager.Core.Enum;

namespace TaskManager.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(20);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(20);
            builder.Property(u => u.Email).IsRequired();
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.Phone).HasMaxLength(20).IsRequired();
            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.UserRole).IsRequired();
            builder.HasQueryFilter(x => !x.IsDeleted);

            builder.HasData(new User
            {
                Id = -1,
                FirstName = "Mohammad",
                LastName = "Alsukhni",
                Email = "admin@taskmanager.local",
                Phone = "0790000000",
                PasswordHash = "$2a$11$Fl8jk/Zy1yA/M84RLJkfqeO1tMea4yc4vLm9yejrjNMszzvNMlUA.",
                UserRole = UserRole.Admin,
                IsActive = true,
                IsDeleted = false,
                CreatedDate = new DateTime(2026, 5, 27),
                CreatedBy = "Seed"
            });

        }
    }
}

