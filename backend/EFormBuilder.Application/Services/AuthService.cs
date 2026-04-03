
using BCrypt.Net;
using EFormBuilder.Application.DTOs.Auth;
using EFormBuilder.Application.Interfaces;
using EFormBuilder.Domain.Entities;
using EFormBuilder.Domain.Exceptions;
using EFormBuilder.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EFormBuilder.Application.Services;
public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _config;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    public AuthService(AppDbContext context, IConfiguration config, ITokenService tokenService, IEmailService emailService)
    {
        _context = context;
        _config = config;
        _tokenService = tokenService;
        _emailService = emailService;
    }

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        // 1. Check tồn tại
        if (await _context.Users.AnyAsync(x => x.Email == request.Email))
        {
            throw new BusinessException(ErrorCode.EmailAlreadyExists);
        }

        string otp = new Random().Next(100000, 999999).ToString();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            OtpCode = otp,
            OtpExpireTime = DateTime.UtcNow.AddMinutes(5),
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // 2. Gửi mail
        await _emailService.SendOtpEmailAsync(user.Email, otp);

        return new RegisterResponse
        {
            Message = "Đăng ký thành công! Vui lòng kiểm tra mã OTP trong Email.",
            Email = _emailService.MaskEmail(user.Email),
            ExpiresInMinutes = 5
        };
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == request.Email);

        // Check mật khẩu
        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new BusinessException(ErrorCode.InvalidCredentials);
        }

        // QUAN TRỌNG: Check xem đã xác thực OTP chưa
        if (!user.IsActive)
        {
            throw new BusinessException(ErrorCode.UserNotActive);
        }

        // Trả về cả cặp Access + Refresh Token như mình đã làm ở bước trước
        return new LoginResponse
        {
            AccessToken = _tokenService.GenerateAccessToken(user),
        };
    }

    public async Task<bool> VerifyOtpAsync(VerifyOtpRequest request)
    {
        // Thay vì dùng email và code rời rạc, ta lấy từ request
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
            throw new BusinessException(ErrorCode.UserNotFound);

        if (user.IsActive)
            throw new BusinessException(ErrorCode.UserAlreadyActive);

        if (user.OtpCode != request.OtpCode) // Lấy từ request
            throw new BusinessException(ErrorCode.InvalidOtp);

        if (user.OtpExpireTime < DateTime.UtcNow)
            throw new BusinessException(ErrorCode.OtpExpired);

        user.IsActive = true;
        user.OtpCode = null;
        user.OtpExpireTime = null;

        await _context.SaveChangesAsync();
        return true;
    }
}