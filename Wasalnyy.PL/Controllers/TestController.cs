using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Wasalnyy.PL.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TestController : ControllerBase
	{
		
		[HttpGet("admin")]
		[Authorize(Roles = "Admin")]
		public IActionResult AdminOnly() => Ok("Only Admins can access");

		[HttpGet("driver")]
		[Authorize(Roles = "Driver")]
		public IActionResult DriverOnly() => Ok("Only Drivers can access");

		[HttpGet("rider")]
		[Authorize(Roles = "Rider")]
		public IActionResult RiderOnly() => Ok("Only Riders can access");
	}
}
