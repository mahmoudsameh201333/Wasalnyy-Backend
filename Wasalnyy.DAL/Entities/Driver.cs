namespace Wasalnyy.DAL.Entities
{
    public class Driver
    {
		public bool IsActive { get; set; }
		public string License { get; set; }
		//public string Location { get; set; } // could be coordinates or string format
		public DateTime? ModifiedAt { get; set; }
		public Vehicle Vehicle { get; set; }
		public List<Review>? Reviews { get; set; } = new List<Review>();
		public List<Trip>? Trips { get; set; } = new List<Trip>();
	}
}
