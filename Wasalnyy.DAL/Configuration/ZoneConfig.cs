namespace Wasalnyy.DAL.Configuration
{
    public class ZoneConfig : IEntityTypeConfiguration<Zone>
    {
        public void Configure(EntityTypeBuilder<Zone> builder)
        {
            builder.Ignore(e => e.Coordinates);

            builder.Property(e => e.Coordinates)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<IEnumerable<Coordinates>>(v))
                .HasColumnType("nvarchar(max)");


            builder.HasIndex(z => new { z.MinLat, z.MaxLat });
            builder.HasIndex(z => new { z.MinLng, z.MaxLng });

            builder.HasIndex(z => z.MinLat);
            builder.HasIndex(z => z.MaxLat);
            builder.HasIndex(z => z.MinLng);
            builder.HasIndex(z => z.MaxLng);

            builder.HasIndex(z => new { z.MinLat, z.MaxLat, z.MinLng, z.MaxLng });
        }
    }
}
