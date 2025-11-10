namespace Wasalnyy.DAL.Configuration
{
    public class TripConfig : IEntityTypeConfiguration<Trip>
    {
        public void Configure(EntityTypeBuilder<Trip> builder)
        {
            builder.HasKey(t => t.Id); 

            builder.Property(t => t.Distination)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(t => t.PickupPoint)
                .IsRequired()
                .HasMaxLength(500);

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

        }
    }
}
