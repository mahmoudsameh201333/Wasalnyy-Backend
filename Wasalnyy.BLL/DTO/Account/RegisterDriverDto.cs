namespace Wasalnyy.BLL.DTO.Account
{
	public class RegisterDriverDto
	{
		public string FullName { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public string Password { get; set; }
		public string License { get; set; }
		public VehicleDto Vehicle { get; set; }
	}
}
