using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;


namespace EFormBuilder.Application.Services;

public interface ICookieService
{
    void SetRefreshCookie(string token);
    void ClearRefreshCookie();
}

public class CookieService : ICookieService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _config;

    public CookieService(IHttpContextAccessor httpContextAccessor, IConfiguration config)
    {
        _httpContextAccessor = httpContextAccessor;
        _config = config;
    }

    public void SetRefreshCookie(string token)
    {
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, // Quan trọng nhất: Chống XSS
            Secure = _config.GetValue<bool>("Jwt:IsSecure"), // Chỉ gửi qua HTTPS
            SameSite = SameSiteMode.Lax, // Chống CSRF
            Path = "/",
            Expires = DateTime.UtcNow.AddDays(_config.GetValue<int>("Jwt:RefreshCookieMaxAgeDays"))
        };

        _httpContextAccessor.HttpContext?.Response.Cookies.Append("refresh_token", token, cookieOptions);
    }

    public void ClearRefreshCookie()
    {
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete("refresh_token");
    }
}