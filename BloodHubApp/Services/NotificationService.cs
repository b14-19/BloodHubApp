using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BloodHubApp.Services
{
    public interface INotificationService
    {
        Task<bool> SendEmailAsync(string to, string subject, string htmlBody);
        Task<bool> SendSmsAsync(string toPhoneE164, string message);
    }

    // Minimal, dependency-free (uses System.Net.Mail). No-ops if not configured.
    public sealed class NotificationService : INotificationService
    {
        private readonly IConfiguration _cfg;
        private readonly ILogger<NotificationService> _log;

        public NotificationService(IConfiguration cfg, ILogger<NotificationService> log)
        {
            _cfg = cfg;
            _log = log;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string htmlBody)
        {
            var host = _cfg["Notifications:Smtp:Host"];
            var from = _cfg["Notifications:Smtp:From"];
            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(from))
            {
                _log.LogInformation("Email not sent: SMTP not configured (Notifications:Smtp:Host/From).");
                return false; // no-op
            }

            var port = int.TryParse(_cfg["Notifications:Smtp:Port"], out var p) ? p : 587;
            var user = _cfg["Notifications:Smtp:User"];
            var pass = _cfg["Notifications:Smtp:Pass"];
            var enableSsl = !bool.TryParse(_cfg["Notifications:Smtp:DisableSsl"], out var noSsl) || !noSsl;

            try
            {
                using var msg = new MailMessage(from, to)
                {
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                using var client = new SmtpClient(host, port)
                {
                    EnableSsl = enableSsl,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };

                if (!string.IsNullOrWhiteSpace(user))
                {
                    client.Credentials = new NetworkCredential(user, pass);
                }

                await client.SendMailAsync(msg);
                _log.LogInformation("Email sent to {To}", to);
                return true;
            }
            catch (System.Exception ex)
            {
                _log.LogWarning(ex, "Email send failed to {To}", to);
                return false;
            }
        }

        // SMS: free options rarely exist. Provide optional email-to-SMS gateway support.
        // If Notifications:Sms:EmailGatewaySuffix is set (e.g., "+<country> carriers"),
        // and phone provided, we send an email to {digits}{suffix}.
        public async Task<bool> SendSmsAsync(string toPhoneE164, string message)
        {
            var suffix = _cfg["Notifications:Sms:EmailGatewaySuffix"]; // e.g., "@txt.att.net"
            if (string.IsNullOrWhiteSpace(suffix))
            {
                _log.LogInformation("SMS not sent: no free SMS configured. Set Notifications:Sms:EmailGatewaySuffix to use email-to-SMS.");
                return false; // no-op
            }

            var digits = Regex.Replace(toPhoneE164 ?? string.Empty, "[^0-9]", "");
            if (string.IsNullOrEmpty(digits))
            {
                _log.LogInformation("SMS not sent: invalid phone.");
                return false;
            }

            var smsAsEmail = digits + suffix;
            return await SendEmailAsync(smsAsEmail, "", WebUtility.HtmlEncode(message));
        }
    }
}
