using EFormBuilder.Application.DTOs.Auth;

namespace EFormBuilder.Application.Interfaces; // Hoặc .Services tùy cấu trúc folder của bạn

public interface IAuthService
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest request);

    Task<LoginResponse> LoginAsync(LoginRequest request);

    Task<bool> VerifyOtpAsync(VerifyOtpRequest request);


    Task<LoginResponse> RefreshTokenAsync(string refreshToken);

    Task LogoutAsync(string refreshToken);

    Task<UserResponse> GetCurrentUserAsync(Guid userId);

    Task ForgotPasswordAsync(ForgotPasswordRequest request);
    Task ResetPasswordAsync(ResetPasswordRequest request);
}