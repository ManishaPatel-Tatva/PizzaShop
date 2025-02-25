using Microsoft.AspNetCore.Mvc;
using BusinessLogicLayer.Helper;
using BusinessLogicLayer.Interfaces;
using System.Threading.Tasks;

public class MailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public MailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("SendMail")]
    public async Task<IActionResult> SendMail([FromBody] EmailRequest emailRequest)
    {
        await _emailService.SendEmailAsync(emailRequest.ToEmail, emailRequest.Subject, emailRequest.Body);
        return Ok("Email sent successfully!");
    }
}

public class EmailRequest
{
    public string ToEmail { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
}