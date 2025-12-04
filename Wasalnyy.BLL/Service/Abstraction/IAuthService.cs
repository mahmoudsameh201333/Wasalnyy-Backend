namespace Wasalnyy.BLL.Service.Abstraction
{
	public interface IAuthService
	{
		Task<AuthResult> LoginAsync(LoginDto dto, string? role);
		Task<AuthResult> GoogleLoginAsync(GoogleLoginDto dto);
		Task<AuthResult> FacebookLoginAsync(FacebookLoginDto dto);
		Task<AuthResult> RegisterDriverAsync(RegisterDriverDto dto);
		Task<AuthResult> RegisterRiderAsync(RegisterRiderDto dto);
		Task<AuthResult> RegisterDriverFaceAsync(string driverId, byte[] faceImage);
		Task<AuthResult> FaceLoginAsync(byte[] faceImage);
		Task<AuthResult> UpdateEmailAsync(string userId, string newEmail);

    }
}
