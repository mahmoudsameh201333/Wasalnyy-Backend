using Microsoft.AspNetCore.Identity;
using Wasalnyy.BLL.JwtHandling;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.BLL.Service.Implementation
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<User> _userManager;
		private readonly JwtHandler _jwtHandler;
		private readonly SignInManager<User> _signInManager;
		public AuthService(UserManager<User> userManager, JwtHandler jwtHandler, SignInManager<User> signInManager)
		{
			_userManager = userManager;
			_jwtHandler = jwtHandler;
			_signInManager = signInManager;
		}
		public async Task<AuthResult> RegisterDriverAsync(RegisterDriverDto dto)
		{
			var driver = new Driver
			{
				UserName = dto.Email,
				Email = dto.Email,
				FullName = dto.FullName,
				PhoneNumber = dto.PhoneNumber,
				License = dto.License,
				Vehicle = new Vehicle
				{
					Make = dto.Vehicle.Make,
					Model = dto.Vehicle.Model,
					PlateNumber = dto.Vehicle.PlateNumber,
					Transmission = dto.Vehicle.Transmission,
					Type = dto.Vehicle.Type,
					EngineType = dto.Vehicle.EngineType,
					Capacity = dto.Vehicle.Capacity,
					Color = dto.Vehicle.Color,
					Year = dto.Vehicle.Year,
				}
			};

			var result = await _userManager.CreateAsync(driver, dto.Password);
			if (!result.Succeeded)
				return new AuthResult { Success = false, Message = string.Join(", ", result.Errors.Select(e => e.Description)) };

			await _userManager.AddToRoleAsync(driver, "Driver");
			var token = await _jwtHandler.GenerateToken(driver);
			return new AuthResult { Success = true, Message = "Driver registered successfully", Token = token };
		}

		public async Task<AuthResult> RegisterRiderAsync(RegisterRiderDto dto)
		{
			var rider = new Rider
			{
				UserName = dto.Email,
				Email = dto.Email,
				FullName = dto.FullName,
				PhoneNumber = dto.PhoneNumber,
				Provider = dto.Provider
			};

			var result = await _userManager.CreateAsync(rider, dto.Password);
			if (!result.Succeeded)
				return new AuthResult
				{
					Success = false,
					Message = string.Join(", ", result.Errors.Select(e => e.Description))
				};

			await _userManager.AddToRoleAsync(rider, "Rider");
			var token = await _jwtHandler.GenerateToken(rider);

			return new AuthResult { Success = true, Message = "Rider registered successfully", Token = token };
		}

		public async Task<AuthResult> LoginAsync(LoginDto dto, string? role = null)
		{
			try
			{
				var user = await _userManager.FindByEmailAsync(dto.Email);
				if (user == null)
					return new AuthResult { Success = false, Message = "Invalid email or password" };
				var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
				if (!result.Succeeded)
					return new AuthResult { Success = false, Message = "Invalid email or password" };
				if (!string.IsNullOrEmpty(role))
				{
					var roles = await _userManager.GetRolesAsync(user);
					if (!roles.Contains(role))
						return new AuthResult { Success = false, Message = $"User is not in the '{role}' role" };
				}
				var token = await _jwtHandler.GenerateToken(user);
				return new AuthResult { Success = true, Message = "Login successful", Token = token };
			}
			catch (Exception ex)
			{
				return new AuthResult { Success = false, Message = ex.Message };
			}
		}
	}
}
