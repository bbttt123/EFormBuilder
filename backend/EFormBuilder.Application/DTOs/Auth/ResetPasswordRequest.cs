using System.ComponentModel.DataAnnotations;

namespace EFormBuilder.Application.DTOs.Auth;

public class ResetPasswordRequest
{
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ")]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = "Mã OTP là bắt buộc")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã OTP phải có đúng 6 ký tự")]
    public string OtpCode { get; set; } = default!;

    [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
    [MinLength(6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự")]
    public string NewPassword { get; set; } = default!;
}