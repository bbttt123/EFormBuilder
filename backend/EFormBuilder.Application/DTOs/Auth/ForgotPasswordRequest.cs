using System.ComponentModel.DataAnnotations;

namespace EFormBuilder.Application.DTOs.Auth;

public class ForgotPasswordRequest
{
    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ")]
    public string Email { get; set; } = default!;
}