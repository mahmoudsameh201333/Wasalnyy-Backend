namespace Wasalnyy.DAL.Entities
{
    public class Zone
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Coordinates> Coordinates { get; set; } = Enumerable.Empty<Coordinates>();
        public decimal MinLat { get; set; }
        public decimal MaxLat { get; set; }
        public decimal MinLng { get; set; }
        public decimal MaxLng { get; set; }
    }
}
