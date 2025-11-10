namespace Wasalnyy.DAL.Configuration
{
	public class VehicleConfig : IEntityTypeConfiguration<Vehicle>
	{
		public void Configure(EntityTypeBuilder<Vehicle> builder)
		{
			builder.Property(v => v.Transmission).HasConversion<string>();
			builder.Property(v => v.Type).HasConversion<string>();
			builder.Property(v => v.Capacity).HasConversion<string>();
			builder.Property(v => v.EngineType).HasConversion<string>();
		}
	}
}
