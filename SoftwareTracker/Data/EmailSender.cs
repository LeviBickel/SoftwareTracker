using Microsoft.AspNetCore.Identity.UI.Services;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace SoftwareTracker.Data
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        public EmailSender(ILogger<EmailSender> logger, ApplicationDbContext context)
        {
            _logger = logger;
        }

        

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var response = await Execute(AkeylessHelper.RetrieveSecret("SendGridAPI"), subject, message, email);
            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                // log or throw
                _logger.LogError("Could not send email: " + await response.Body.ReadAsStringAsync());
                
            }
        }

        private async Task<Response> Execute(string apiKey, string subject, string message, string email)
        {
            var client = new SendGridClient(apiKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress("bickelsolutions@gmail.com", "Software Tracker Admin"),
                Subject = subject,
                PlainTextContent = message,
                HtmlContent = message
            };
            msg.AddTo(new EmailAddress(email));

            msg.SetClickTracking(false, false);

            return await client.SendEmailAsync(msg);
        }
    }
}
