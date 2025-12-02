namespace Wasalnyy.BLL.DTO.Driver
{
    public class ReturnDriverDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public Guid? ZoneId { get; set; }
        public string License { get; set; }
        public VehicleDto Vehicle { get; set; }
        public Coordinates? Coordinates { get; set; }
        public string DriverStatus { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
