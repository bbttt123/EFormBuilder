namespace EFormBuilder.Application.DTOs.Auth;

public class VerifyOtpRequest
{
    public string Email { get; set; } = default!;
    public string OtpCode { get; set; } = default!;
}