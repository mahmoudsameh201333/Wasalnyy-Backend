using Microsoft.AspNetCore.Identity;

namespace Wasalnyy.BLL.JwtHandling
{
	public class JwtHandler
	{
		private readonly JwtSettings _jwtSettings;
		private readonly UserManager<User> _userManager;
		public JwtHandler(IOptions<JwtSettings> setting, UserManager<User> userManager)
		{
			_jwtSettings = setting.Value;
			_userManager = userManager;
		}
		public async Task<string> GenerateToken(User user)
		{
			var roles = await _userManager.GetRolesAsync(user);
			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Id),
				new Claim(JwtRegisteredClaimNames.Email, user.Email!),
				new Claim(ClaimTypes.Name, user.FullName),
				new Claim(ClaimTypes.NameIdentifier, user.Id)
			};

			foreach (var role in roles)
				claims.Add(new Claim(ClaimTypes.Role, role));

			var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwtSettings.Key));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _jwtSettings.Issuer,
				audience: _jwtSettings.Audience,
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
