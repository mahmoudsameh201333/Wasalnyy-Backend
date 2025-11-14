using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.DAL.Entities;
using Wasalnyy.DAL.Enum;

namespace Wasalnyy.BLL.DTO.Trip
{
    public class RequestTripDto
    {
        public PaymentMethod PaymentMethod { get; set; }
        public IEnumerable<Coordinates> PickupCoordinates { get; set; } = Enumerable.Empty<Coordinates>();
        public IEnumerable<Coordinates> DistinationCoordinates { get; set; } = Enumerable.Empty<Coordinates>();
    }
}
