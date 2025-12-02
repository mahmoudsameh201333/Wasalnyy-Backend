namespace Wasalnyy.DAL.Configuration
{
    public class WasalnyyHubConnectionConfig : IEntityTypeConfiguration<WasalnyyHubConnection>
    {
        public void Configure(EntityTypeBuilder<WasalnyyHubConnection> builder)
        {
            builder.HasKey(x => new {x.UserId, x.SignalRConnectionId });

            builder.HasIndex(x => x.SignalRConnectionId)
                   .IsUnique();

            builder.HasIndex(x => x.UserId)
                   .IsUnique(false);
        }
    }
}
