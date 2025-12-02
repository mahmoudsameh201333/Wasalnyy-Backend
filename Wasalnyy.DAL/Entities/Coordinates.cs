namespace Wasalnyy.DAL.Entities
{
    [Owned]
    public class Coordinates
    {
        [Required]
        public decimal Lat { get; set; }

        [Required]
        public decimal Lng { get; set; }
    }
}
