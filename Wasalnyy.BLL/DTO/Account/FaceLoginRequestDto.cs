using Microsoft.AspNetCore.Http;

namespace Wasalnyy.BLL.DTO.Account
{
	public class FaceLoginRequestDto
	{
		public IFormFile FaceImage { get; set; }
	}
}
