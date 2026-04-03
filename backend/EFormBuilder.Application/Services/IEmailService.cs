namespace EFormBuilder.Application.Interfaces;

public interface IEmailService
{
    Task SendOtpEmailAsync(string toEmail, string otpCode);
    string MaskEmail(string email);
}