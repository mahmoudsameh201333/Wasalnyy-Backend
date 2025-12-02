namespace Wasalnyy.BLL.DTO.Trip
{
    public class RequestTripDto
    {
        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public Coordinates PickupCoordinates { get; set; }

        [Required]
        public Coordinates DestinationCoordinates { get; set; }
        public string? PickUpName { get; set; }
        public string? DestinationName { get; set; }
    }
}
