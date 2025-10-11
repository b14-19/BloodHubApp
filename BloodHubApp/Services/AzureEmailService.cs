using Azure;
using Azure.Communication.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BloodHubApp.Services
{
    /// <summary>
    /// Production-ready email service using Azure Communication Services
    /// FREE tier: 500 emails/month
    /// Secure: No passwords, uses connection string
    /// </summary>
    public class AzureEmailService : INotificationService
    {
        private readonly IConfiguration _cfg;
        private readonly ILogger<AzureEmailService> _log;

        public AzureEmailService(IConfiguration cfg, ILogger<AzureEmailService> log)
        {
            _cfg = cfg;
            _log = log;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string htmlBody)
        {
            var connectionString = _cfg["AzureCommunication:ConnectionString"];
            var senderAddress = _cfg["AzureCommunication:SenderAddress"];

            if (string.IsNullOrWhiteSpace(connectionString) || string.IsNullOrWhiteSpace(senderAddress))
            {
                _log.LogInformation("Email not sent: Azure Communication Services not configured");
                return false;
            }

            try
            {
                var emailClient = new EmailClient(connectionString);

                var emailContent = new EmailContent(subject)
                {
                    Html = htmlBody
                };

                var emailMessage = new EmailMessage(
                    senderAddress: senderAddress,
                    recipientAddress: to,
                    content: emailContent);

                EmailSendOperation emailSendOperation = await emailClient.SendAsync(
                    WaitUntil.Completed,
                    emailMessage);

                _log.LogInformation("Email sent to {To}. Message ID: {MessageId}", to, emailSendOperation.Id);
                return true;
            }
            catch (Exception ex)
            {
                _log.LogWarning(ex, "Email send failed to {To}", to);
                return false;
            }
        }

        public async Task<bool> SendSmsAsync(string toPhoneE164, string message)
        {
            // SMS requires Azure Communication Services Phone Numbers (not free)
            // For production, integrate with Twilio or similar service
            _log.LogInformation("SMS not implemented. Use Twilio for production SMS.");
            return await Task.FromResult(false);
        }
    }
}
