using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wasalnyy.DAL.Enum;


namespace Wasalnyy.DAL.Entities
{
	public class Vehicle
	{
		[Key]
		public string PlateNumber { get; set; }
		public string Model { get; set; }
		public string Color { get; set; }
		public VehicleType Type { get; set; }
		public string Make { get; set; }
		public DateOnly Year { get; set; }
		public Capacity Capacity { get; set; }
		public Transmission Transmission { get; set; }
		public EngineType EngineType { get; set; }

		public string DriverId { get; set; }
		public Driver Driver { get; set; }

	}
}
