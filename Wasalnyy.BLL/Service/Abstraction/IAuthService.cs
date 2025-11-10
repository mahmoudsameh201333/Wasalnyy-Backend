namespace Wasalnyy.BLL.Service.Abstraction
{
	public interface IAuthService
	{
		Task<AuthResult> LoginAsync(LoginDto dto, string? role);
		Task<AuthResult> RegisterDriverAsync(RegisterDriverDto dto);
		Task<AuthResult> RegisterRiderAsync(RegisterRiderDto dto);
	}
}
