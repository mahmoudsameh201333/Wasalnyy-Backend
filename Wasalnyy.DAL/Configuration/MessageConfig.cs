using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.DAL.Configuration
{
    public class MessageConfig : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            // Primary key
            builder.HasKey(m => m.Id);

            // Configure SenderId foreign key
            builder.HasOne(m => m.Sender)
                   .WithMany()
                   .HasForeignKey(m => m.SenderId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configure ReceiverId foreign key
            builder.HasOne(m => m.Receiver)
                   .WithMany()
                   .HasForeignKey(m => m.ReceiverId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Configure properties
            builder.Property(m => m.SenderId)
                   .IsRequired();

            builder.Property(m => m.ReceiverId)
                   .IsRequired();

            builder.Property(m => m.Content)
                   .IsRequired()
                   .HasMaxLength(2000);

            builder.Property(m => m.SentAt)
                   .IsRequired();

            builder.Property(m => m.IsRead)
                   .IsRequired()
                   .HasDefaultValue(false);

            builder.Property(m => m.ReadAt)
                   .IsRequired(false);

            // Indexes for better query performance
            builder.HasIndex(m => m.SenderId);
            builder.HasIndex(m => m.ReceiverId);
            builder.HasIndex(m => m.SentAt);
            builder.HasIndex(m => new { m.SenderId, m.ReceiverId, m.SentAt });
        }
    }
}