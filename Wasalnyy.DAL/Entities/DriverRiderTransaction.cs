namespace Wasalnyy.DAL.Entities
{
    public class DriverRiderTransaction
    {
        public Guid Id { get; set; }
        public string DriverId { get; set; }
        public Driver Driver { get; set; }
        public string RiderId { get; set; }
        public Rider Rider { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}