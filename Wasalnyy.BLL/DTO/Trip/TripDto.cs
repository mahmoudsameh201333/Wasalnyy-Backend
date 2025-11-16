using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wasalnyy.BLL.DTO.Trip
{
    public class TripDto
    {
        public Guid Id { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public TripStatus TripStatus { get; set; } = TripStatus.Requested;
        public double DistanceKm { get; set; }
        public double DurationMinutes { get; set; }
        public double Price { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public Coordinates PickupCoordinates { get; set; }
        public Coordinates DistinationCoordinates { get; set; }
        public string? DriverId { get; set; }
        public string RiderId { get; set; }
        public Guid ZoneId { get; set; }
    }
}
