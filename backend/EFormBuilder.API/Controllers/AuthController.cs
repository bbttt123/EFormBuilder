using EFormBuilder.Application.DTOs;
using EFormBuilder.Application.DTOs.Auth;
using EFormBuilder.Application.Interfaces;
using EFormBuilder.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace EFormBuilder.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            // Trả về ApiResponse chuẩn Success
            return Ok(ApiResponse<RegisterResponse>.SuccessResponse(result));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            return Ok(ApiResponse<LoginResponse>.SuccessResponse(result));
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var isSuccess = await _authService.VerifyOtpAsync(request);

            return Ok(ApiResponse<string>.SuccessResponse("Xác thực tài khoản thành công!"));
        }
    }
}