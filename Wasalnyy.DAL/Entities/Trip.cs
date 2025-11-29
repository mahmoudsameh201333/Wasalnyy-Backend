namespace Wasalnyy.DAL.Entities
{
	public class Trip
	{
		public Guid Id { get; set; }
		public PaymentMethod PaymentMethod { get; set; }
		public TripStatus TripStatus { get; set; } = TripStatus.Requested;
		public double DistanceKm { get; set; }
		public double DurationMinutes { get; set; }
		public double Price { get; set; }

		public DateTime RequestedDate { get; set; }
		public DateTime? StartDate { get; set; }
        public DateTime? ArrivalDate { get; set; }


        public Coordinates? CurrentCoordinates { get; set; }
        public Coordinates PickupCoordinates { get; set; }
        public Coordinates DistinationCoordinates { get; set; }
		public string? PickUpName { get; set; }
		public string? DestinationName { get; set; }
		public string? DriverId { get; set; }
		public Driver? Driver { get; set; }
		public string RiderId { get; set; }
		public Rider Rider { get; set; }

        public Guid ZoneId { get; set; }
        public Zone Zone{ get; set; }

        public List<Review> Reviews { get; set; } = new List<Review>();
	}
}
