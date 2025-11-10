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


	}
}
