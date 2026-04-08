using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using EFormBuilder.Application.Interfaces;

namespace EFormBuilder.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    public EmailService(IConfiguration config) => _config = config;

    public async Task SendOtpEmailAsync(string toEmail, string otpCode, string subject = "Mã xác thực OTP")
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("E-Form Builder", _config["EmailSettings:SenderEmail"]));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject; // Tiêu đề mail thay đổi theo mục đích

        email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = $@"
            <div style='font-family: sans-serif; text-align: center; border: 1px solid #eee; padding: 20px; border-radius: 10px;'>
                <h2 style='color: #007bff;'>{subject}</h2>
                <p>Xin chào, mã OTP của bạn là:</p>
                <div style='background: #f8f9fa; padding: 15px; display: inline-block; border-radius: 5px; margin: 10px 0;'>
                    <b style='font-size: 32px; letter-spacing: 5px; color: #333;'>{otpCode}</b>
                </div>
                <p>Mã này có hiệu lực trong ít phút. Vui lòng không chia sẻ cho bất kỳ ai.</p>
                <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'>
                <small style='color: #999;'>Đây là email tự động từ hệ thống E-Form Builder.</small>
            </div>"
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_config["EmailSettings:Username"], _config["EmailSettings:Password"]);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }

    public string MaskEmail(string email)
    {
        if (string.IsNullOrEmpty(email) || !email.Contains("@"))
            return email;

        var parts = email.Split('@');
        var name = parts[0];
        var domain = parts[1];

        // Logic: Giữ lại 3 ký tự đầu, còn lại thay bằng ***
        if (name.Length > 3)
        {
            return name.Substring(0, 3) + "***@" + domain;
        }

        // Nếu tên email quá ngắn (vd: a@gmail.com)
        return name[0] + "***@" + domain;
    }
}