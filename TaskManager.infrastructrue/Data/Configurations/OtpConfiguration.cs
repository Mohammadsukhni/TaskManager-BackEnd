using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Core.Entities;

namespace TaskManager.Infrastructure.Data.Configurations
{
    public class OtpConfiguration : IEntityTypeConfiguration<Otp>
    {
        public void Configure(EntityTypeBuilder<Otp> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedOnAdd();
            builder.Property(x => x.ReceiverId).IsRequired();
            builder.Property(x => x.Code).IsRequired().HasMaxLength(6);
            builder.Property(x => x.ActionType).IsRequired();
            builder.HasQueryFilter(x => !x.IsDeleted);

            builder.HasOne(x => x.Receiver)
                .WithMany(x => x.Otps)
                .HasForeignKey(x => x.ReceiverId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
