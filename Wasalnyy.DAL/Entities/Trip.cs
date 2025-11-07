using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.DAL.Enum;

namespace Wasalnyy.DAL.Entities
{
	public class Trip
	{
		public Guid Id { get; set; }
		public PaymentMethod PaymentMethod { get; set; }
		public RideStatus Status { get; set; } = RideStatus.Pending;
		public double Distance { get; set; }
		public string Distination { get; set; }
		public string PickupPoint { get; set; }
		public double Price { get; set; }

		public DateTime Duration;
		public DateTime ArrivalDate { get; set; }
		public DateTime StartDate { get; set; }
		public double StartLat { get; set; }
		public double StartLng { get; set; }
		public double EndLat { get; set; }
		public double EndLng { get; set; }

		public string DriverId { get; set; }
		public Driver Driver { get; set; }
		public string RiderId { get; set; }
		public Rider Rider { get; set; }

		public List<Review> Reviews { get; set; } = new List<Review>();
	}
}
