using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.DAL.Configuration
{
    public class GatewayPaymentConfig : IEntityTypeConfiguration<GatewayPayment>
    {
        public void Configure(EntityTypeBuilder<GatewayPayment> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.TransactionId)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(p => p.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(p => p.Status)
                .IsRequired();

            builder.Property(p => p.Date)
                .IsRequired();

            builder.HasOne(p => p.Rider)
                .WithMany()
                .HasForeignKey(p => p.RiderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(p => p.RiderId);
            builder.HasIndex(p => p.TransactionId);
            builder.HasIndex(p => p.Date);
            builder.HasIndex(p => new { p.RiderId, p.Date });
        }
    }
}