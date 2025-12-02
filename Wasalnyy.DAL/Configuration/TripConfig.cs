namespace Wasalnyy.DAL.Configuration
{
    public class TripConfig : IEntityTypeConfiguration<Trip>
    {
        public void Configure(EntityTypeBuilder<Trip> builder)
        {
            builder.HasKey(t => t.Id);


            builder.OwnsOne(e => e.CurrentCoordinates, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.Property(c => c.Lat)
                    .HasConversion<decimal>()
                    .HasColumnName("CurrentLat")
                    .IsRequired();

                ownedNavigationBuilder.Property(c => c.Lng)
                    .HasConversion<decimal>()
                    .HasColumnName("CurrentLng")
                    .IsRequired();
            });

            builder.OwnsOne(e => e.PickupCoordinates, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.Property(c => c.Lat)
                    .HasConversion<decimal>()
                    .HasColumnName("PickupLat")
                    .IsRequired();

                ownedNavigationBuilder.Property(c => c.Lng)
                    .HasConversion<decimal>()
                    .HasColumnName("PickupLng")
                    .IsRequired();
            });

            builder.OwnsOne(e => e.DestinationCoordinates, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.Property(c => c.Lat)
                    .HasConversion<decimal>()
                    .HasColumnName("DestinationLat")
                    .IsRequired();

                ownedNavigationBuilder.Property(c => c.Lng)
                    .HasConversion<decimal>()
                    .HasColumnName("DestinationLng")
                    .IsRequired();
            });

            builder.HasOne(t => t.Driver)
                .WithMany(d => d.Trips)
                .HasForeignKey(t => t.DriverId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(t => t.Rider)
                .WithMany(r => r.Trips)
                .HasForeignKey(t => t.RiderId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(t => t.Zone)
                .WithMany()
                .HasForeignKey(t => t.ZoneId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Property(e => e.TripStatus)
                .HasConversion<string>();

            builder.Property(e => e.PaymentMethod)
                .HasConversion<string>();

            builder.HasIndex(t => t.DriverId);

            builder.HasIndex(t => t.RiderId);

            builder.HasIndex(t => t.ZoneId);

            builder.HasIndex(t => t.TripStatus);

            builder.HasIndex(t => new { t.ZoneId, t.TripStatus });

            builder.HasIndex(t => new { t.DriverId, t.TripStatus });

            builder.HasIndex(t => new { t.RiderId, t.TripStatus });

            builder.HasIndex(t => t.RequestedDate);
        }
    }
}
