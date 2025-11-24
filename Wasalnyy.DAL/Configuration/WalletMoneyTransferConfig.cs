using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.DAL.Configuration
{
    public class WalletMoneyTransferConfig : IEntityTypeConfiguration<WalletMoneyTransfer>
    {
        public void Configure(EntityTypeBuilder<WalletMoneyTransfer> builder)
        {
            builder.ToTable("WalletMoneyTransfers");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Amount)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            // Sender Wallet
            builder.HasOne(t => t.SenderWallet)
                .WithMany()
                .HasForeignKey(t => t.SenderWalletId)
                .OnDelete(DeleteBehavior.Restrict);

            // Receiver Wallet
            builder.HasOne(t => t.ReceiverWallet)
                .WithMany()
                .HasForeignKey(t => t.ReceiverWalletId)
                .OnDelete(DeleteBehavior.Restrict);

            // Trip (optional)
            builder.HasIndex(t => t.TripId);

            // Useful Indexes
            builder.HasIndex(t => t.SenderWalletId);
            builder.HasIndex(t => t.ReceiverWalletId);
            builder.HasIndex(t => t.CreatedAt);

            builder.HasIndex(t => new { t.SenderWalletId, t.CreatedAt });
            builder.HasIndex(t => new { t.ReceiverWalletId, t.CreatedAt });
        }
    }
}
