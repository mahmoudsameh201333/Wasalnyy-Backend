using Wasalnyy.DAL.Entities;

namespace Wasalnyy.DAL.Database
{
    public class WasalnyyDbContext: IdentityDbContext<User>
    {
        public WasalnyyDbContext(DbContextOptions<WasalnyyDbContext> options) : base(options)
        {
        }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

            builder.Entity<Review>().HasKey(x=> new { x.RiderId, x.DriverId, x.TripId, x.ReviewerType });
		}

        public DbSet<User> Users {  get; set; }
        public DbSet<Rider> Riders {  get; set; }
        public DbSet<Driver> Drivers {  get; set; }
        public DbSet<Vehicle> Vehicles {  get; set; }
        public DbSet<Review> Reviews {  get; set; }
        public DbSet<Zone> Zones {  get; set; }


    }
}
