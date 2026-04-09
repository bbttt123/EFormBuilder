using System.ComponentModel.DataAnnotations;

namespace EFormBuilder.Application.DTOs.Auth;

public class LoginRequest
{
    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ")]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = "Mật khẩu không được để trống")]
    public string Password { get; set; } = default!;
}