using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Wasalnyy.BLL.Helper;
using Wasalnyy.DAL.Entities;
using Wasalnyy.DAL.Repo.Abstraction;

namespace Wasalnyy.BLL.Service.Implementation
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<User> _userManager;
		private readonly JwtHandler _jwtHandler;
		private readonly SignInManager<User> _signInManager;
		private readonly IEmailService _emailService;
		private readonly string BaseUrl;
		private readonly IFaceService _faceService;
		private readonly IUserFaceDataRepo _faceRepo;
		public AuthService(
			UserManager<User> userManager,
			JwtHandler jwtHandler,
			SignInManager<User> signInManager,
			IEmailService emailService,
			IConfiguration config,
			IFaceService faceService,
			IUserFaceDataRepo faceRepo)
		{
			_userManager = userManager;
			_jwtHandler = jwtHandler;
			_signInManager = signInManager;
			_emailService = emailService;
			BaseUrl = config["BaseUrl"]!;
			_faceRepo = faceRepo;
			_faceService = faceService;
		}
		public async Task<AuthResult> RegisterDriverAsync(RegisterDriverDto dto)
		{
			var existingUser = await _userManager.FindByEmailAsync(dto.Email);
			if (existingUser != null)
			{
				return new AuthResult
				{
					Success = false,
					Message = "Email is already registered"
				};
			}
			var licenseExists = _userManager.Users.OfType<Driver>().Any(d => d.License == dto.License);
			if (licenseExists)
				return new AuthResult { Success = false, Message = "License is already registered" };

			var plateExists = _userManager.Users.OfType<Driver>().Any(d => d.Vehicle.PlateNumber == dto.Vehicle.PlateNumber);
			if (plateExists)
				return new AuthResult { Success = false, Message = "Vehicle plate number already exists" };

			if (dto.Vehicle.Year < 1990 || dto.Vehicle.Year > DateTime.Now.Year)
				return new AuthResult { Success = false, Message = "Invalid vehicle year" };

			var driver = new Driver
			{
				UserName = dto.Email,
				Email = dto.Email,
				FullName = dto.FullName,
				PhoneNumber = dto.PhoneNumber,
				License = dto.License,
				CreatedAt = DateTime.UtcNow,
				DateOfBirth = dto.DateOfBirth,
				Gender = dto.Gender,
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

			var token = await _userManager.GenerateEmailConfirmationTokenAsync(driver);
			var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
			var confirmationLink = $"{BaseUrl}/api/auth/confirm-email?userId={driver.Id}&token={encodedToken}";
			await _emailService.SendEmail(
				driver.Email,
				"Confirm your email",
				$"Please confirm your account: <a href='{confirmationLink}'>Click here</a>"
			);

			await _userManager.AddToRoleAsync(driver, "Driver");
			var jwt_token = await _jwtHandler.GenerateToken(driver);
			return new AuthResult { Success = true, Message = "Driver registered successfully", Token = jwt_token, DriverId = driver.Id };
		}

		public async Task<AuthResult> RegisterRiderAsync(RegisterRiderDto dto)
		{
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return new AuthResult
                {
                    Success = false,
                    Message = "Email is already registered"
                };
            }
            var rider = new Rider
			{
				UserName = dto.Email,
				Email = dto.Email,
				FullName = dto.FullName,
				PhoneNumber = dto.PhoneNumber,
				CreatedAt = DateTime.UtcNow,
				DateOfBirth = dto.DateOfBirth,
				Gender = dto.Gender,
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
				"WasalnyyUber App  ---- Confirm your email",
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
		public async Task<AuthResult> RegisterDriverFaceAsync(string driverId, byte[] faceImage)
		{
			var driver = await _userManager.FindByIdAsync(driverId);
			if (driver == null)
				return new AuthResult { Success = false, Message = "Driver not found" };
			double[] embedding;
			try
			{
				embedding = _faceService.ExtractEmbedding(faceImage);
			}
			catch
			{
				return new AuthResult { Success = false, Message = "No face detected" };
			}
			var faceData = new UserFaceData
			{
				DriverId = driver.Id,
				Embedding = EmbeddingSerializer.DoubleArrayToBytes(embedding)
			};
			await _faceRepo.AddAsync(faceData);
			return new AuthResult { Success = true, Message = "Face registered successfully" };
		}
		public async Task<AuthResult> FaceLoginAsync(byte[] faceImage)
		{
			double[] incomingEmbedding;
			try
			{
				incomingEmbedding = _faceService.ExtractEmbedding(faceImage);
			}
			catch
			{
				return new AuthResult { Success = false, Message = "No face detected in image" };
			}

			var drivers = await _faceRepo.GetAllDriversAsync();
			User? matchedUser = null;
			double bestDistance = double.MaxValue;
			foreach (var d in drivers)
			{
				double[] storedEmbedding = EmbeddingSerializer.BytesToDoubleArray(d.Embedding);
				double distance = _faceService.CompareEmbeddings(storedEmbedding, incomingEmbedding);
				if (distance < bestDistance)
				{
					bestDistance = distance;
					matchedUser = d.Driver;
				}
			}
			const double THRESHOLD = 0.6; // tune this
			if (matchedUser != null && bestDistance <= THRESHOLD)
			{
				await _signInManager.SignInAsync(matchedUser, isPersistent: false);
				var token = await _jwtHandler.GenerateToken(matchedUser);
				return new AuthResult { Success = true, Message = "Face login successful", Token = token };
			}
			return new AuthResult { Success = false, Message = "Face not recognized" };
		}
	}
}
