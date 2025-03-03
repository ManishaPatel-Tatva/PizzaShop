using MailKit.Security;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

using PizzaShop.Entity.Models;
using PizzaShop.Service.Interfaces;
using PizzaShop.Service.Configuration;
using Microsoft.Extensions.Logging;

namespace PizzaShop.Service.Services;

public class EmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress( _emailSettings.FromEmail, _emailSettings.FromEmail));
        email.To.Add(new MailboxAddress(toEmail,toEmail));
        email.Subject = subject;
        email.Body = new TextPart("html"){Text=body};

        using var smtp = new SmtpClient();
        try
        {
            await smtp.ConnectAsync(_emailSettings.Host,_emailSettings.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailSettings.UserName, _emailSettings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            _logger.LogInformation($"Email sent successfully to {toEmail}");
        }
        catch(Exception e){
             _logger.LogError($"Error sending email: {e.Message}");
        }
    }
}