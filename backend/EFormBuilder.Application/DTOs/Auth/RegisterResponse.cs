namespace EFormBuilder.Application.DTOs.Auth;

public class RegisterResponse
{
    public string Message { get; set; } = string.Empty;

    // Trả về email đã mask (ngu***@gmail.com) để FE hiển thị thông báo
    public string Email { get; set; } = string.Empty;

    // Để FE biết mã này sống được bao lâu mà hiển thị bộ đếm ngược (countdown)
    public int ExpiresInMinutes { get; set; }
}