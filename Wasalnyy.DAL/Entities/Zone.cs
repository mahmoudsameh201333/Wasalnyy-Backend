namespace Wasalnyy.DAL.Entities
{
    public class Zone
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<Coordinate> Coordinates { get; set; } = Enumerable.Empty<Coordinate>();
        public decimal MinLat { get; set; }
        public decimal MaxLat { get; set; }
        public decimal MinLng { get; set; }
        public decimal MaxLng { get; set; }
    }
}
