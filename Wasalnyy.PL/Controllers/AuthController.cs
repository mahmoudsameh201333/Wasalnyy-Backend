using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
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

		[HttpPost("google-login")]
		public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
		{
			var result = await _authService.GoogleLoginAsync(dto);
			if (!result.Success)
				return BadRequest(result);
			return Ok(result);
		}

		[HttpPost("facebook-login")]
		public async Task<IActionResult> FacebookLogin([FromBody] FacebookLoginDto dto)
		{
			if (dto == null || string.IsNullOrEmpty(dto.AccessToken))
				return BadRequest(new { Success = false, Message = "Invalid request" });
			var result = await _authService.FacebookLoginAsync(dto);
			if (!result.Success)
				return BadRequest(result);
			return Ok(result);
		}

		[HttpPost("register/driver")]
		public async Task<IActionResult> RegisterDriver([FromBody] RegisterDriverDto dto)
		{
			var result = await _authService.RegisterDriverAsync(dto);
			if (!result.Success)
				return BadRequest(result);
			return Ok(result);
		}

		[HttpPost("register/rider")]
		public async Task<IActionResult> RegisterRider([FromBody] RegisterRiderDto dto)
		{
			var result = await _authService.RegisterRiderAsync(dto);
			if (!result.Success)
				return BadRequest(result.Message);
			return Ok(new { result.Message, result.Token });
		}


		
		[HttpPost("register/driver-face")]
        [Consumes("multipart/form-data")]

        public async Task<IActionResult> RegisterDriverFace([FromForm] RegisterDriverFaceRequestDto model)
		{
			if (model.FaceImage == null || model.FaceImage.Length == 0)
				return BadRequest("Face image is required.");

			using var ms = new MemoryStream();
			await model.FaceImage.CopyToAsync(ms);

			var result = await _authService.RegisterDriverFaceAsync(model.DriverId, ms.ToArray());

			if (!result.Success)
				return BadRequest(result.Message);

			return Ok(new { message=result.Message });
		}
		[HttpPost("login/driver-face")]
        [Consumes("multipart/form-data")]
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

        [HttpPost("update-email")]
        public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized("Invalid token");
            var result = await _authService.UpdateEmailAsync(userId, dto.NewEmail);

            if (!result.Success)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

    }
}
