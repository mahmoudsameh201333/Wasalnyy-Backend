using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Google.Apis.Auth;
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
		private readonly IWalletRepo _walletRepo;
		private readonly IWalletService _walletService;
		private readonly HttpClient _httpClient;
		public AuthService(
			UserManager<User> userManager,
			JwtHandler jwtHandler,
			SignInManager<User> signInManager,
			IEmailService emailService,
			IConfiguration config,
			IFaceService faceService,
			IUserFaceDataRepo faceRepo,
			IWalletRepo walletRepo,
			IWalletService walletService,
			HttpClient httpClient)
		{
			_userManager = userManager;
			_jwtHandler = jwtHandler;
			_signInManager = signInManager;
			_emailService = emailService;
			BaseUrl = config["BaseUrl"]!;
			_faceRepo = faceRepo;
			_walletRepo = walletRepo;
			_faceService = faceService;
			_walletService = walletService;
			_httpClient = httpClient;
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
			var confirmationLink = $"{BaseUrl}/api/Email/confirm-email?userId={driver.Id}&token={encodedToken}";
			await _emailService.SendEmail(
				driver.Email,
				"Confirm your email",
				$"Please confirm your account: <a href='{confirmationLink}'>Click here</a>"
			);

			await _userManager.AddToRoleAsync(driver, "Driver");
			var jwt_token = await _jwtHandler.GenerateToken(driver);

			var resp = await _walletService.CreateWalletAsync(new DTO.Wallet.CreateWalletDTO
			{
				Balance = 0,
				UserId = driver.Id,

				CreatedAt = DateTime.Now

			});
			if (!resp.IsSuccess)
			{
				return new AuthResult { Success = false, Message = "Driver registered but wallet creation failed: " + resp.Message };
			}
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
			var confirmationLink = $"{BaseUrl}/api/Email/confirm-email?userId={rider.Id}&token={encodedToken}";
			await _emailService.SendEmail(
				rider.Email,
				"WasalnyyUber App  ---- Confirm your email",
				$"Please confirm your account: <a href='{confirmationLink}'>Click here</a>"
			);

			await _userManager.AddToRoleAsync(rider, "Rider");
			var jwt_token = await _jwtHandler.GenerateToken(rider);

			var resp = await _walletService.CreateWalletAsync(new DTO.Wallet.CreateWalletDTO
			{
				Balance = 0,
				UserId = rider.Id,

				CreatedAt = DateTime.Now

			});
			if (!resp.IsSuccess)
			{
				return new AuthResult { Success = false, Message = "Rider registered but wallet creation failed: " + resp.Message };
			}

			return new AuthResult { Success = true, Message = "Rider registered successfully", Token = jwt_token };
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
				if (user.IsSuspended)
				{
					return new AuthResult { Success = false, Message = "Your account has been suspended. Please contact support." };
				}
				if(user.IsDeleted)
				{
					return new AuthResult { Success = false, Message = "Your account has been deleted. Please contact support." };
				}
				var token = await _jwtHandler.GenerateToken(user);
				return new AuthResult { Success = true, Message = "Login successful", Token = token };
			}
			catch (Exception ex)
			{
				return new AuthResult { Success = false, Message = ex.Message };
			}
		}
		public async Task<AuthResult> GoogleLoginAsync(GoogleLoginDto dto)
		{
			try
			{
				var payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdToken);
				if (payload == null)
					return new AuthResult { Success = false, Message = "Invalid Google token" };
				var user = await _userManager.FindByEmailAsync(payload.Email);
				if (user == null)
				{
					user = new Rider
					{
						UserName = payload.GivenName,
						Email = payload.Email,
						FullName = payload.Name,
						PhoneNumber = "",
						ProviderId = payload.Subject,
						Image = payload.Picture,
						Provider = "Google",
						CreatedAt = DateTime.UtcNow
					};
					var createResult = await _userManager.CreateAsync(user);
					if (!createResult.Succeeded)
						return new AuthResult { Success = false, Message = "Could not create user: " + string.Join(", ", createResult.Errors.Select(e => e.Description)) };

					await _userManager.AddToRoleAsync(user, "Rider");
					await _walletService.CreateWalletAsync(new CreateWalletDTO
					{
						Balance = 0,
						UserId = user.Id,
						CreatedAt = DateTime.Now
					});
				}
				if (user.IsSuspended)
					return new AuthResult { Success = false, Message = "Your account is suspended." };
				if (user.IsDeleted)
					return new AuthResult { Success = false, Message = "Your account was deleted." };

				var token = await _jwtHandler.GenerateToken(user);
				return new AuthResult { Success = true, Message = "Google login successful", Token = token };
			}
			catch (Exception ex)
			{
				return new AuthResult { Success = false, Message = ex.Message };
			}
		}
		public async Task<AuthResult> FacebookLoginAsync(FacebookLoginDto dto)
		{
			try
			{
				var facebookUser = await ValidateFacebookTokenAsync(dto.AccessToken);
				if (facebookUser == null || string.IsNullOrEmpty(facebookUser.Email))
					return new AuthResult { Success = false, Message = "Invalid Facebook token or email not provided" };
				var user = await _userManager.FindByEmailAsync(facebookUser.Email);
				if (user == null)
				{
					// Create new user
					user = new Rider
					{
						UserName = facebookUser.Name.Replace(" ", "_").ToLower(),
						Email = facebookUser.Email,
						FullName = facebookUser.Name,
						PhoneNumber = "",
						Provider = "Facebook",
						ProviderId = facebookUser.Id,
						CreatedAt = DateTime.UtcNow
					};
					var createResult = await _userManager.CreateAsync(user);
					if (!createResult.Succeeded)
						return new AuthResult
						{
							Success = false,
							Message = "Could not create user: " + string.Join(", ", createResult.Errors.Select(e => e.Description))
						};
					await _userManager.AddToRoleAsync(user, "Rider");
					await _walletService.CreateWalletAsync(new CreateWalletDTO
					{
						Balance = 0,
						UserId = user.Id,
						CreatedAt = DateTime.Now,
					});
				}
				if (user.IsSuspended)
					return new AuthResult { Success = false, Message = "Your account is suspended." };

				if (user.IsDeleted)
					return new AuthResult { Success = false, Message = "Your account was deleted." };

				var token = await _jwtHandler.GenerateToken(user);
				return new AuthResult
				{
					Success = true,
					Message = "Facebook login successful",
					Token = token
				};
			}
			catch (Exception ex)
			{
				return new AuthResult { Success = false, Message = ex.Message };
			}
		}

		private async Task<FacebookUserInfoDto> ValidateFacebookTokenAsync(string accessToken)
		{
			try
			{
				var response = await _httpClient.GetAsync( $"https://graph.facebook.com/me?fields=id,name,email,picture.type(large)&access_token={accessToken}");
				if (!response.IsSuccessStatusCode)
					return null!;
				var content = await response.Content.ReadAsStringAsync();
				var userInfo = JsonSerializer.Deserialize<FacebookUserInfoDto>(content, new JsonSerializerOptions{ PropertyNameCaseInsensitive = true});
				return userInfo!;
			}
			catch
			{
				return null!;
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
		public async Task CreateWalletForUserAsync(User user)
		{
			// prevent admin from getting wallet
			if (await _userManager.IsInRoleAsync(user, "Admin"))
				return;

			// Check if wallet already exists
			var existingWallet = await _walletRepo.GetWalletOfUserIdAsync(user.Id);
			if (existingWallet != null)
				return;

			var wallet = new Wallet
			{
				UserId = user.Id,
				Balance = 0,
			};

			await _walletRepo.CreateAsync(wallet);
			await _walletRepo.SaveChangesAsync();
		}
    

	public async Task<AuthResult> UpdateEmailAsync(string userId, string newEmail)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
				return new AuthResult { Success = false, Message = "User not found" };

			// Check if the new email already exists
			var existingUser = await _userManager.FindByEmailAsync(newEmail);
			if (existingUser != null && existingUser.Id != userId)
			{
				return new AuthResult { Success = false, Message = "Email is already taken" };
			}

			// Update the email (unconfirmed)
			user.Email = newEmail;
			user.UserName = newEmail;
			user.NormalizedEmail = newEmail.ToUpper();
			user.EmailConfirmed = false;

			await _userManager.UpdateAsync(user);

			// Generate new confirmation token
			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

			var encodedToken = WebEncoders.Base64UrlEncode(
				Encoding.UTF8.GetBytes(token)
			);

			// Confirmation link
			string confirmUrl =
				$"{BaseUrl}/api/Email/confirm-email?userId={user.Id}&token={encodedToken}";

			// Send confirmation email
			await _emailService.SendEmail(
				newEmail,
				"Confirm your new email",
				$"Click to confirm your new email: <a href='{confirmUrl}'>Confirm Email</a>"
			);

			return new AuthResult
			{
				Success = true,
				Message = "Email updated! Please confirm the new email to activate it."
			};
		}
	}
}
