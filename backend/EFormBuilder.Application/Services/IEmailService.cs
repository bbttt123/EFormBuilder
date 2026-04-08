namespace EFormBuilder.Application.Interfaces;

public interface IEmailService
{
    Task SendOtpEmailAsync(string toEmail, string otpCode, string subject = "Mã xác thực OTP");
    string MaskEmail(string email);
}