namespace Wasalnyy.BLL.DTO.Trip
{
    public class TripDto
    {
        public Guid Id { get; set; }
        public string PaymentMethod { get; set; }
        public string TripStatus { get; set; }
        public double DistanceKm { get; set; }
        public double DurationMinutes { get; set; }
        public double Price { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public Coordinates? CurrentCoordinates { get; set; }
        public Coordinates PickupCoordinates { get; set; }
        public Coordinates DistinationCoordinates { get; set; }
        public string? DriverId { get; set; }
        public string RiderId { get; set; }
        public Guid ZoneId { get; set; }
		public string? PickUpName { get; set; }
		public string? DestinationName { get; set; }
	}
}
