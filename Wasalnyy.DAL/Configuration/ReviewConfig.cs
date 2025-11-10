namespace Wasalnyy.DAL.Configuration
{
	internal class ReviewConfig : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
			builder.HasIndex(r => new { r.RiderId, r.DriverId, r.TripId, r.ReviewerType })
				   .IsUnique();

			builder.HasOne(r => r.Rider)
				   .WithMany(r => r.Reviews)
				   .HasForeignKey(r => r.RiderId)
				   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(r => r.Driver)
                .WithMany(d => d.Reviews)
                .HasForeignKey(r => r.DriverId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(r => r.Rider)
                .WithMany(r => r.Reviews)
                .HasForeignKey(r => r.RiderId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(r => r.Trip)
                .WithMany(t => t.Reviews)
                .HasForeignKey(r => r.TripId)
				   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
