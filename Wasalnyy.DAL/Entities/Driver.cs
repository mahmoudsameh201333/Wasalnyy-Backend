namespace Wasalnyy.DAL.Entities
{
    public class Driver : User
    {
		public DriverStatus DriverStatus { get; set; }
		public string License { get; set; }
		public DateTime? ModifiedAt { get; set; }
		public Vehicle Vehicle { get; set; }
		public List<Review>? Reviews { get; set; } = new List<Review>();
		public List<Trip>? Trips { get; set; } = new List<Trip>();
        public Guid? ZoneId { get; set; }
        public Zone? Zone { get; set; }
        public Coordinates? Coordinates { get; set; }
    }
}
