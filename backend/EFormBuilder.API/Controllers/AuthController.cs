using EFormBuilder.Application.DTOs;
using EFormBuilder.Application.DTOs.Auth;
using EFormBuilder.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            await _authService.VerifyOtpAsync(request);
            return Ok(ApiResponse<string>.SuccessResponse("Xác thực tài khoản thành công!"));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            // Lấy refresh_token từ HttpOnly Cookie
            var refreshToken = Request.Cookies["refresh_token"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                // Nếu không có cookie, trả về lỗi 401
                return Unauthorized(ApiResponse<object>.Failure("AUTH_002", "Không tìm thấy phiên đăng nhập"));
            }

            var result = await _authService.RefreshTokenAsync(refreshToken);
            return Ok(ApiResponse<LoginResponse>.SuccessResponse(result));
        }

        // --- MỚI: API LOGOUT ---
        [Authorize] // Bắt buộc phải có Access Token để logout
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Lấy refresh_token từ cookie để xóa trong DB
            var refreshToken = Request.Cookies["refresh_token"];

            // Lấy UserId từ Claims của Access Token hiện tại
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(userIdStr, out Guid userId))
            {
                // Gọi service để xóa token trong DB và xóa cookie trình duyệt
                await _authService.LogoutAsync(refreshToken!);
                return Ok(ApiResponse<string>.SuccessResponse("Đăng xuất thành công!"));
            }

            return BadRequest(ApiResponse<object>.Failure("SYS_002", "Yêu cầu không hợp lệ"));
        }


        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized(ApiResponse<object>.Failure("AUTH_001", "Phiên làm việc không hợp lệ hoặc đã hết hạn"));
            }

            var userResponse = await _authService.GetCurrentUserAsync(userId);

            return Ok(ApiResponse<UserResponse>.SuccessResponse(userResponse));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            // 1. Gọi service để tạo OTP và gửi mail
            await _authService.ForgotPasswordAsync(request);

            // 2. Luôn trả về Success để bảo mật (tránh lộ việc email có tồn tại hay không)
            return Ok(ApiResponse<object>.SuccessResponse("Mã OTP đã được gửi đến Email của bạn."));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            // 1. Gọi service để kiểm tra OTP và đổi mật khẩu
            await _authService.ResetPasswordAsync(request);

            return Ok(ApiResponse<object>.SuccessResponse("Đặt lại mật khẩu thành công!"));
        }
    }

}