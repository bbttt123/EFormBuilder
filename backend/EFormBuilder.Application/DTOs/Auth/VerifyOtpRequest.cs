using System.ComponentModel.DataAnnotations;

namespace EFormBuilder.Application.DTOs.Auth;

public class VerifyOtpRequest
{
    [Required(ErrorMessage = "Email không được để trống")]
    [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ")]
    public string Email { get; set; } = default!;

    [Required(ErrorMessage = "Mã OTP không được để trống")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã OTP phải có đúng 6 ký tự")]
    public string OtpCode { get; set; } = default!;
}