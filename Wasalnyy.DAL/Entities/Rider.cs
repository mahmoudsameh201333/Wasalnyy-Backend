namespace Wasalnyy.DAL.Entities
{
    public class Rider : User
    {
		public string? ProviderId { get; set; }
		public string? Provider { get; set; }
		public DateTime? ModifiedAt { get; set; }
		public List<Review>? Reviews { get; set; } = new List<Review>();
		
		public List<Trip>? Trips { get; set; } = new List<Trip>();

	}
}
