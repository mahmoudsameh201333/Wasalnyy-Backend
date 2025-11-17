using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Wasalnyy.PL.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginDto dto, [FromQuery] string? role)
		{
			var result = await _authService.LoginAsync(dto, role);

			if (!result.Success)
				return Unauthorized(result.Message);

			return Ok(new { result.Message, result.Token });
		}

		[HttpPost("register/driver")]
		public async Task<IActionResult> RegisterDriver([FromBody] RegisterDriverDto dto)
		{
			var result = await _authService.RegisterDriverAsync(dto);

			if (!result.Success)
				return BadRequest(result.Message);

			return Ok(new { result.Message, result.Token });
		}

		[HttpPost("register/rider")]
		public async Task<IActionResult> RegisterRider([FromBody] RegisterRiderDto dto)
		{
			var result = await _authService.RegisterRiderAsync(dto);

			if (!result.Success)
				return BadRequest(result.Message);

			return Ok(new { result.Message, result.Token });
		}
		[HttpGet("confirm-email")]
		public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
		{
			var result = await _authService.ConfirmEmailAsync(userId, token);

			if (!result.Success)
				return BadRequest(result.Message);

			return Ok(result.Message);
		}
		[HttpPost("register/driver-face")]
		public async Task<IActionResult> RegisterDriverFace([FromForm] RegisterDriverFaceRequestDto model)
		{
			if (model.FaceImage == null || model.FaceImage.Length == 0)
				return BadRequest("Face image is required.");

			using var ms = new MemoryStream();
			await model.FaceImage.CopyToAsync(ms);

			var result = await _authService.RegisterDriverFaceAsync(model.DriverId, ms.ToArray());

			if (!result.Success)
				return BadRequest(result.Message);

			return Ok(result.Message);
		}
		[HttpPost("login/driver-face")]
		public async Task<IActionResult> FaceLogin([FromForm] FaceLoginRequestDto model)
		{
			if (model.FaceImage == null || model.FaceImage.Length == 0)
				return BadRequest("Face image is required.");

			using var ms = new MemoryStream();
			await model.FaceImage.CopyToAsync(ms);

			var result = await _authService.FaceLoginAsync(ms.ToArray());

			if (!result.Success)
				return Unauthorized(result.Message);

			return Ok(new { result.Message, result.Token });
		}
	}
}
