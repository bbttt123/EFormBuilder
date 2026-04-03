

using EFormBuilder.Application.DTOs.Auth;

namespace EFormBuilder.Application.Services;

public interface IAuthService
{
    Task<RegisterResponse> RegisterAsync(RegisterRequest request);
    Task<LoginResponse> LoginAsync(LoginRequest request);

    Task<bool> VerifyOtpAsync(VerifyOtpRequest request);
}