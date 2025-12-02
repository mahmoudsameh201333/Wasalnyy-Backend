using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.DAL.Configuration
{
    public class ChatHubConnectionConfig : IEntityTypeConfiguration<ChatHubConnection>
    {
        public void Configure(EntityTypeBuilder<ChatHubConnection> builder)
        {
            // Composite primary key
            builder.HasKey(x => new { x.UserId, x.SignalRConnectionId });

            // Configure UserId as foreign key
            builder.HasOne(x => x.User)
                   .WithMany()
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Configure column properties
            builder.Property(x => x.UserId)
                   .IsRequired();

            builder.Property(x => x.SignalRConnectionId)
                   .IsRequired();
        }
    }
}