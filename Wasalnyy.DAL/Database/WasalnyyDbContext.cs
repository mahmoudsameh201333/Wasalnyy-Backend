using Microsoft.EntityFrameworkCore;

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

            builder.Entity<Wallet>()
               .HasOne(w => w.User)
               .WithOne()                     // Rider or Driver (inherit from User)
               .HasForeignKey<Wallet>(w => w.UserId)
               .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<WalletTransactionLogs>()
                .HasOne(t => t.Wallet)
                .WithMany(w => w.Transactions)
                .HasForeignKey(t => t.WalletId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Trip>()
                .Property(e => e.TripStatus)
                .HasConversion<string>();
            
            builder.Entity<Driver>()
                .Property(e => e.DriverStatus)
                .HasConversion<string>();
        }


        public DbSet<User> Users {  get; set; }
        public DbSet<Rider> Riders {  get; set; }
        public DbSet<Driver> Drivers {  get; set; }
        public DbSet<Vehicle> Vehicles {  get; set; }
        public DbSet<Review> Reviews {  get; set; }
        public DbSet<Trip> Trips {  get; set; }
        public DbSet<Zone> Zones {  get; set; }
		public DbSet<UserFaceData> UserFaceData { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletTransactionLogs> WalletTransactions { get; set; }
        public DbSet<WalletMoneyTransfer> DriverRiderTransactions { get; set; }
        public DbSet<WasalnyyHubConnection> WasalnyyHubConnections {  get; set; }
        public DbSet<GatewayPaymentTransactions> GatewayPayments { get; set; }

    }
}
