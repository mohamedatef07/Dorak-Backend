using System.Net;
using System.Net.Mail;
using Dorak.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Services.EmailService;

namespace Dorak.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IEmailSender _emailSender;

        public EmailController(IConfiguration config, IEmailSender emailSender)
        {
            _config = config;
            _emailSender = emailSender;
        }

        [HttpPost("SendEmail")]
        public async Task<IActionResult> SendEmail([FromBody] ContactFormModel form)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                using var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(
                        _config["Smtp:Username"],
                        _config["Smtp:Password"]
                    ),
                    EnableSsl = true
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(_config["Smtp:From"], "Dorak Contact Form"),
                    Subject = string.IsNullOrWhiteSpace(form.Subject)
                        ? "New Contact Request"
                        : form.Subject,
                    Body = $@"
                        <p><strong>From:</strong> {form.Name} ({form.Email})</p>
                        {(string.IsNullOrWhiteSpace(form.Phone) ? "" : $"<p><strong>Phone:</strong> {form.Phone}</p>")}
                        {(string.IsNullOrWhiteSpace(form.CenterName) ? "" : $"<p><strong>Center:</strong> {form.CenterName}</p>")}
                        {(string.IsNullOrWhiteSpace(form.InquiryType) ? "" : $"<p><strong>Inquiry Type:</strong> {form.InquiryType}</p>")}
                        <p><strong>Message:</strong></p>
                        <p>{form.Message}</p>
                    ",
                    IsBodyHtml = true
                };

                mail.ReplyToList.Add(new MailAddress(form.Email));

                mail.To.Add(_config["Smtp:To"]);

                await smtpClient.SendMailAsync(mail);

                return Ok(new { message = "Email sent successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "Failed to send email",
                    details = ex.Message
                });
            }
        }

        [HttpPost("test")]
        public async Task<IActionResult> TestEmail()
        {
            await _emailSender.SendEmailAsync(
                "dorak.helpdesk@gmail.com",
                "Test from Dorak EmailSender",
                "<p>This is a test message from Dorak's email service.</p>"
            );
            return Ok("Test email sent!");
        }
    }
}