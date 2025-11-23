using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.DAL.Configuration
{
    public class DriverRiderTransactionConfig : IEntityTypeConfiguration<DriverRiderTransaction>
    {
        public void Configure(EntityTypeBuilder<DriverRiderTransaction> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.HasOne(t => t.Driver)
                .WithMany()
                .HasForeignKey(t => t.DriverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(t => t.Rider)
                .WithMany()
                .HasForeignKey(t => t.RiderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(t => t.DriverId);
            builder.HasIndex(t => t.RiderId);
            builder.HasIndex(t => t.CreatedAt);
            builder.HasIndex(t => new { t.DriverId, t.CreatedAt });
            builder.HasIndex(t => new { t.RiderId, t.CreatedAt });
        }
    }
}