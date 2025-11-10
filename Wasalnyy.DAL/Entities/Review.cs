namespace Wasalnyy.DAL.Entities
{
	public class Review
	{
		public Guid Id { get; set; }
		public string DriverId { get; set; }
		public Driver Driver { get; set; }
		public string RiderId { get; set; }
		public Rider Rider { get; set; }
		public Guid TripId { get; set; }
		public Trip Trip { get; set; }
		public string? Comment { get; set; }
		public int? Stars { get; set; }
		public ReviewerType ReviewerType { get; set; }
	}
}
