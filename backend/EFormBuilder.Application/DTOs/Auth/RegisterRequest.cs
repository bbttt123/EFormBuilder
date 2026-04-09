using System.ComponentModel.DataAnnotations;

namespace EFormBuilder.Application.DTOs.Auth;

public class RegisterRequest
{
    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    public string Password { get; set; } = string.Empty;
}