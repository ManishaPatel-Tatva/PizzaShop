using MailKit.Security;
using MailKit.Net.Smtp;
using MimeKit;

using PizzaShop.Service.Interfaces;
using Microsoft.Extensions.Logging;
using PizzaShop.Service.Configuration;

namespace PizzaShop.Service.Services;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;

    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }

    public async Task<bool> SendEmail(string toEmail, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(EmailConfig.FromName, EmailConfig.FromEmail));
        email.To.Add(new MailboxAddress(toEmail,toEmail));
        email.Subject = subject;
        email.Body = new TextPart("html"){Text=body};

        using var smtp = new SmtpClient();
        try
        {
            await smtp.ConnectAsync(EmailConfig.Host,EmailConfig.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(EmailConfig.UserName, EmailConfig.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            _logger.LogInformation($"Email sent successfully to {toEmail}");
            return true;
        }
        catch(Exception e){
             _logger.LogError($"Error sending email: {e.Message}");
             return false;
        }
    }
}