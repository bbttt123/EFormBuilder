using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;
using EFormBuilder.Application.Interfaces;

namespace EFormBuilder.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    public EmailService(IConfiguration config) => _config = config;

    public async Task SendOtpEmailAsync(string toEmail, string otpCode)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress("E-Form Builder", _config["EmailSettings:SenderEmail"]));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = "Mã xác thực OTP của bạn";

        // Giao diện HTML cho Email nhìn cho chuyên nghiệp
        email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
        {
            Text = $@"
                <div style='font-family: sans-serif; text-align: center; border: 1px solid #eee; padding: 20px;'>
                    <h2 style='color: #007bff;'>Xác thực đăng ký</h2>
                    <p>Mã OTP của bạn là:</p>
                    <b style='font-size: 30px; letter-spacing: 5px; color: #333;'>{otpCode}</b>
                    <p>Mã có hiệu lực trong 5 phút. Đừng chia sẻ cho ai nhé!</p>
                </div>"
        };

        using var smtp = new SmtpClient();
        // Kết nối tới Google
        await smtp.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
        // Đăng nhập bằng App Password bạn đã lấy
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