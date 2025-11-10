namespace Wasalnyy.DAL.Entities
{
	public class Trip
	{
		public Guid Id { get; set; }
		public PaymentMethod PaymentMethod { get; set; }
		public TripStatus TripStatus { get; set; } = TripStatus.Requested;
		public double Distance { get; set; }
		public string Distination { get; set; }
		public string PickupPoint { get; set; }
		public decimal Price { get; set; }

		[NotMapped]
		public double Duration { get =>  ArrivalDate.Subtract( StartDate).TotalMinutes;  }
		public DateTime ArrivalDate { get; set; }
		public DateTime StartDate { get; set; }
		public double StartLat { get; set; }
		public double StartLng { get; set; }
		public double EndLat { get; set; }
		public double EndLng { get; set; }

		public string DriverId { get; set; }
		public Driver Driver { get; set; }
		public string RiderId { get; set; }
		public Rider Rider { get; set; }

        public Guid ZoneId { get; set; }
        public Zone Zone{ get; set; }

        public List<Review> Reviews { get; set; } = new List<Review>();
	}
}
