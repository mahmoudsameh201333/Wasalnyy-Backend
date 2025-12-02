using System.Text.Json.Serialization;

namespace Wasalnyy.DAL.Entities
{
    [Owned]
    public class Coordinates
    {
        [Required]
        [JsonPropertyName("Lat")]
        public decimal Lat { get; set; }

        [Required]
        [JsonPropertyName("Lng")]
        public decimal Lng { get; set; }
    }
}
