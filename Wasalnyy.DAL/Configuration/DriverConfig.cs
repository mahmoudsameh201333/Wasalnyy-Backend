namespace Wasalnyy.DAL.Configuration
{
    internal class DriverConfig : IEntityTypeConfiguration<Driver>
    {
        public void Configure(EntityTypeBuilder<Driver> builder)
        {
            builder.OwnsOne(e => e.Coordinates, ownedNavigationBuilder =>
            {
                ownedNavigationBuilder.Property(c => c.Lat)
                    .HasConversion<decimal>()
                    .HasColumnName("Lat")
                    .IsRequired();

                ownedNavigationBuilder.Property(c => c.Lng)
                    .HasConversion<decimal>()
                    .HasColumnName("Lng")
                    .IsRequired();

                ownedNavigationBuilder.HasIndex(c => c.Lat);
                ownedNavigationBuilder.HasIndex(c => c.Lng);
                ownedNavigationBuilder.HasIndex(c => new { c.Lat, c.Lng });
            });


            builder.Property(e => e.DriverStatus)
                .HasConversion<string>();

            builder.HasIndex(d => new { d.ZoneId, d.DriverStatus });

            builder.HasIndex(d => d.License)
                .IsUnique();


            builder.HasIndex(d => d.IsDeleted);
        }
    }
}
