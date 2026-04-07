

using EFormBuilder.Application.DTOs.Auth;
using EFormBuilder.Application.Interfaces;
using EFormBuilder.Domain.Entities;
using EFormBuilder.Domain.Exceptions;
using EFormBuilder.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace EFormBuilder.Application.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly ICookieService _cookieService; // Inject thêm cái này

    public AuthService(
        AppDbContext context,
        ITokenService tokenService,
        IEmailService emailService,
        ICookieService cookieService)
    {
        _context = context;
        _tokenService = tokenService;
        _emailService = emailService;
        _cookieService = cookieService;
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
        await _emailService.SendOtpEmailAsync(user.Email, otp, "Xác thực đăng ký tài khoản");

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

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new BusinessException(ErrorCode.InvalidCredentials);

        if (!user.IsActive)
            throw new BusinessException(ErrorCode.UserNotActive);

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshTokenValue = _tokenService.GenerateRefreshToken();

        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshTokenValue,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsUsed = false,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(refreshTokenEntity);
        await _context.SaveChangesAsync();

        _cookieService.SetRefreshCookie(refreshTokenValue);

        return new LoginResponse { AccessToken = accessToken };
    }

    public async Task<bool> VerifyOtpAsync(VerifyOtpRequest request)
    {
       
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
            throw new BusinessException(ErrorCode.UserNotFound);

        if (user.IsActive)
            throw new BusinessException(ErrorCode.UserAlreadyActive);

        if (user.OtpCode != request.OtpCode) 
            throw new BusinessException(ErrorCode.InvalidOtp);

        if (user.OtpExpireTime < DateTime.UtcNow)
            throw new BusinessException(ErrorCode.OtpExpired);

        user.IsActive = true;
        user.OtpCode = null;
        user.OtpExpireTime = null;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<LoginResponse> RefreshTokenAsync(string token)
    {
        var storedToken = await _context.RefreshTokens
            .Include(u => u.User)
            .FirstOrDefaultAsync(t => t.Token == token);

        // 1. Kiểm tra tồn tại & Hết hạn
        if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow || storedToken.IsRevoked)
        {
            throw new BusinessException(ErrorCode.InvalidRefreshToken);
        }

        // 2. Phát hiện Token Reuse 
        if (storedToken.IsUsed)
        {
            // Nếu token đã dùng rồi mà vẫn gửi lên 
            // Thu hồi TẤT CẢ token của user này để đảm bảo an toàn
            var allUserTokens = await _context.RefreshTokens
                .Where(t => t.UserId == storedToken.UserId)
                .ToListAsync();

            allUserTokens.ForEach(t => t.IsRevoked = true);
            await _context.SaveChangesAsync();

            throw new BusinessException(ErrorCode.TokenCompromised);
        }

        // 3. Mark token cũ là đã sử dụng
        storedToken.IsUsed = true;

        // 4. Sinh cặp mới (Rotation)
        var newAccessToken = _tokenService.GenerateAccessToken(storedToken.User);
        var newRefreshTokenValue = _tokenService.GenerateRefreshToken();

        var newRefreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = newRefreshTokenValue,
            UserId = storedToken.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsUsed = false,
            IsRevoked = false
        };

        _context.RefreshTokens.Add(newRefreshTokenEntity);
        await _context.SaveChangesAsync();

        _cookieService.SetRefreshCookie(newRefreshTokenValue);

        return new LoginResponse { AccessToken = newAccessToken };
    }
    public async Task LogoutAsync(string token)
    {
        var storedToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == token);

        if (storedToken != null)
        {
            storedToken.IsRevoked = true; 
            await _context.SaveChangesAsync();
        }

        _cookieService.ClearRefreshCookie();
    }

    public async Task<UserResponse> GetCurrentUserAsync(Guid userId)
    {
        var user = await _context.Users
            .AsNoTracking() 
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new BusinessException(ErrorCode.UserNotFound);
        }

        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null) return;

        string otp = new Random().Next(100000, 999999).ToString();
        user.OtpCode = otp;
        user.OtpExpireTime = DateTime.UtcNow.AddMinutes(15);

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        // Gửi mail thật luôn vì bạn đã có Service rồi
        await _emailService.SendOtpEmailAsync(user.Email, otp, "Khôi phục mật khẩu của bạn");
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        // 1. Kiểm tra User tồn tại
        if (user == null)
        {
            throw new BusinessException(ErrorCode.UserNotFound);
        }

        // 2. Kiểm tra OTP (Đúng mã và chưa hết hạn)
        if (user.OtpCode != request.OtpCode)
        {
            throw new BusinessException(ErrorCode.InvalidOtp);
        }

        if (user.OtpExpireTime < DateTime.UtcNow)
        {
            throw new BusinessException(ErrorCode.OtpExpired);
        }

        // 3. Hash mật khẩu mới và cập nhật
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);

        // 4. Reset các trường OTP để bảo mật (tránh dùng lại mã cũ)
        user.OtpCode = null;
        user.OtpExpireTime = null;
        user.UpdatedAt = DateTime.UtcNow;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }
}