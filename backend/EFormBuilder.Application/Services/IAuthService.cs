

using EFormBuilder.Application.DTOs.Auth;

namespace EFormBuilder.Application.Services;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterRequest request);
    Task<string> LoginAsync(LoginRequest request);
}