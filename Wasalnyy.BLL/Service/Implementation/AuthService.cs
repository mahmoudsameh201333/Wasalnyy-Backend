using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System;
using Wasalnyy.BLL.JwtHandling;
using Wasalnyy.DAL.Entities;

namespace Wasalnyy.BLL.Service.Implementation
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<User> _userManager;
		private readonly JwtHandler _jwtHandler;
		private readonly SignInManager<User> _signInManager;
		private readonly IEmailService _emailService;
		private readonly string BaseUrl;

		public AuthService(
			UserManager<User> userManager,
			JwtHandler jwtHandler,
			SignInManager<User> signInManager,
			IEmailService emailService,
			IConfiguration config)
		{
			_userManager = userManager;
			_jwtHandler = jwtHandler;
			_signInManager = signInManager;
			_emailService = emailService;
			BaseUrl = config["BaseUrl"]!;
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
			
			// Generate email confirmation token
			var token = await _userManager.GenerateEmailConfirmationTokenAsync(rider);
			var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
			var confirmationLink = $"{BaseUrl}/api/auth/confirm-email?userId={rider.Id}&token={encodedToken}";
			await _emailService.SendEmail(
				rider.Email,
				"Confirm your email",
				$"Please confirm your account: <a href='{confirmationLink}'>Click here</a>"
			);

			await _userManager.AddToRoleAsync(rider, "Rider");
			var jwt_token = await _jwtHandler.GenerateToken(rider);

			return new AuthResult { Success = true, Message = "Rider registered successfully", Token = jwt_token };
		}
		public async Task<AuthResult> ConfirmEmailAsync(string userId, string token)
		{
			if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
				return new AuthResult { Success = false, Message = "Invalid email confirmation request" };

			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
				return new AuthResult { Success = false, Message = "User not found" };

			var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
			var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

			if (!result.Succeeded)
				return new AuthResult { Success = false, Message = "Email confirmation failed" };

			return new AuthResult { Success = true, Message = "Email confirmed successfully!" };
		}
		public async Task<AuthResult> LoginAsync(LoginDto dto, string? role = null)
		{
			try
			{
				var user = await _userManager.FindByEmailAsync(dto.Email);
				if (user == null)
					return new AuthResult { Success = false, Message = "Invalid email or password" };

				if (!await _userManager.IsEmailConfirmedAsync(user))
					return new AuthResult { Success = false, Message = "Please confirm your email before logging in." };

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
