using Microsoft.AspNetCore.Http;

namespace Wasalnyy.BLL.DTO.Account
{
	public class RegisterDriverFaceRequestDto
	{
		public string DriverId { get; set; }
		public IFormFile FaceImage { get; set; }
	}
}
