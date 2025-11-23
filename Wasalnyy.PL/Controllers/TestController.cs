using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Wasalnyy.PL.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class TestController : ControllerBase
	{
        private readonly IConfiguration _configuration;
        public TestController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("admin")]
		[Authorize(Roles = "Admin")]
		public IActionResult AdminOnly() => Ok("Only Admins can access");

		[HttpGet("driver")]
		[Authorize(Roles = "Driver")]
		public IActionResult DriverOnly() => Ok("Only Drivers can access");

		[HttpGet("rider")]
		[Authorize(Roles = "Rider")]
		public IActionResult RiderOnly() => Ok("Only Riders can access");

        [Authorize(Roles = "Driver")]
        [HttpPost("test-login")]
        public IActionResult TestLogin([FromBody] LoginDto model)
        {
            // ANY fake credentials (you decide)
            if (model.Email != "test@driver.com" || model.Password != "123456")
                return Unauthorized("Invalid test credentials");

            // Fake driver ID for testing
            string driverId = "123-test-driver";

            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, driverId),
        new Claim(ClaimTypes.Email, model.Email),
        new Claim(ClaimTypes.Role, "Driver")
    };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = tokenString });
        }
    }
}
