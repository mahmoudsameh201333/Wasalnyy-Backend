namespace Wasalnyy.BLL.DTO
{
	public class VehicleDto
	{
		public string PlateNumber { get; set; }
		public string Model { get; set; }
		public string Color { get; set; }
		public VehicleType Type { get; set; }
		public string Make { get; set; }
		public int Year { get; set; }
		public Capacity Capacity { get; set; }
		public Transmission Transmission { get; set; }
		public EngineType EngineType { get; set; }
	}
}
