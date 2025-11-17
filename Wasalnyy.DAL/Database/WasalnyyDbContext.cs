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
			builder.ApplyConfigurationsFromAssembly(typeof(WasalnyyDbContext).Assembly);
		}

        public DbSet<User> Users {  get; set; }
        public DbSet<Rider> Riders {  get; set; }
        public DbSet<Driver> Drivers {  get; set; }
        public DbSet<Vehicle> Vehicles {  get; set; }
        public DbSet<Review> Reviews {  get; set; }
        public DbSet<Trip> Trips {  get; set; }
        public DbSet<Zone> Zones {  get; set; }
		public DbSet<UserFaceData> UserFaceData { get; set; }

    }
}
