using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BloodHub.Data;
using BloodHubApp.Services;
using System.Text;

namespace BloodHub.Controllers
{
    public class TestController : Controller
    {
        private readonly BloodHubContext _db;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TestController> _logger;

        public TestController(
            BloodHubContext db,
            INotificationService notificationService,
            IConfiguration configuration,
            ILogger<TestController> logger)
        {
            _db = db;
            _notificationService = notificationService;
            _configuration = configuration;
            _logger = logger;
        }

        // Access: https://your-app.azurewebsites.net/Test/Verify
        public async Task<IActionResult> Verify()
        {
            var result = new StringBuilder();
            result.AppendLine("=== BLOODHUB NOTIFICATION SYSTEM VERIFICATION ===\n");

            // 1. Check Azure Communication Services Configuration
            result.AppendLine("1. AZURE COMMUNICATION SERVICES:");
            var acsConnectionString = _configuration["AzureCommunication:ConnectionString"];
            var acsSenderAddress = _configuration["AzureCommunication:SenderAddress"];

            result.AppendLine($"   Connection String: {(string.IsNullOrEmpty(acsConnectionString) ? "NOT SET" : "CONFIGURED")}");
            result.AppendLine($"   Sender Address: {(string.IsNullOrEmpty(acsSenderAddress) ? "NOT SET" : acsSenderAddress)}");
            result.AppendLine();

            // Check old SMTP config (should not be used)
            var smtpHost = _configuration["Notifications:Smtp:Host"];
            if (!string.IsNullOrEmpty(smtpHost))
            {
                result.AppendLine("   ?? Old SMTP configuration detected (will be ignored)");
                result.AppendLine();
            }

            // 2. Check Blood Banks
            result.AppendLine("2. BLOOD BANKS WITH EMAIL:");
            var bloodBanks = await _db.Hospitals
                .Include(h => h.Stock)
                .Where(h => h.IsActive && h.Email != null)
                .ToListAsync();

            if (!bloodBanks.Any())
            {
                result.AppendLine("   NO BLOOD BANKS FOUND");
            }
            else
            {
                result.AppendLine($"   Found {bloodBanks.Count} blood banks:");
                foreach (var bb in bloodBanks)
                {
                    result.AppendLine($"   - {bb.Name}");
                    result.AppendLine($"     Email: {bb.Email}");
                    result.AppendLine($"     City: {bb.City}");
                    if (bb.Stock != null)
                    {
                        result.AppendLine($"     Stock: A+={bb.Stock.APos}, B+={bb.Stock.BPos}, O+={bb.Stock.OPos}");
                    }
                    else
                    {
                        result.AppendLine($"     Stock: NO INVENTORY");
                    }
                    result.AppendLine();
                }
            }

            // 3. Check Narasimha Blood Bank specifically
            result.AppendLine("3. NARASIMHA BLOOD BANK:");
            var narasimha = await _db.Hospitals
                .Include(h => h.Stock)
                .FirstOrDefaultAsync(h => h.Email == "narasimhasez@gmail.com");

            if (narasimha == null)
            {
                result.AppendLine("   NOT FOUND IN DATABASE");
                result.AppendLine("   ACTION: Run the SQL insert script");
            }
            else
            {
                result.AppendLine($"   Found: {narasimha.Name}");
                result.AppendLine($"   Email: {narasimha.Email}");
                result.AppendLine($"   Active: {narasimha.IsActive}");
                if (narasimha.Stock != null)
                {
                    result.AppendLine($"   Stock: A+={narasimha.Stock.APos}, A-={narasimha.Stock.ANeg}");
                    result.AppendLine($"          B+={narasimha.Stock.BPos}, B-={narasimha.Stock.BNeg}");
                    result.AppendLine($"          O+={narasimha.Stock.OPos}, O-={narasimha.Stock.ONeg}");
                    result.AppendLine($"          AB+={narasimha.Stock.ABPos}, AB-={narasimha.Stock.ABNeg}");
                }
                else
                {
                    result.AppendLine("   NO INVENTORY DATA");
                }
            }
            result.AppendLine();

            // 4. Check Recent Blood Requests
            result.AppendLine("4. RECENT BLOOD REQUESTS:");
            var recentRequests = await _db.BloodRequests
                .OrderByDescending(r => r.RequestedAt)
                .Take(3)
                .ToListAsync();

            if (!recentRequests.Any())
            {
                result.AppendLine("   NO REQUESTS FOUND");
            }
            else
            {
                foreach (var req in recentRequests)
                {
                    result.AppendLine($"   Request #{req.Id}");
                    result.AppendLine($"   Blood Group: {req.BloodGroup}, Units: {req.Units}");
                    result.AppendLine($"   Status: {req.Status}");
                    result.AppendLine($"   Requested: {req.RequestedAt:yyyy-MM-dd HH:mm:ss}");
                    result.AppendLine();
                }
            }

            // 5. Test Email Functionality
            result.AppendLine("5. EMAIL TEST:");
            if (string.IsNullOrEmpty(acsConnectionString) || string.IsNullOrEmpty(acsSenderAddress))
            {
                result.AppendLine("   SKIPPED - Azure Communication Services not configured");
                result.AppendLine("   ACTION: See AZURE_COMMUNICATION_SETUP.md");
            }
            else
            {
                try
                {
                    var testResult = await _notificationService.SendEmailAsync(
                        to: "narasimhasez@gmail.com",
                        subject: "BloodHub Test Email - Azure Communication Services",
                        htmlBody: @"<html><body style='font-family: Arial; padding: 20px;'>
                            <h1>Test Successful!</h1>
                            <p>Your BloodHub application is now using <strong>Azure Communication Services</strong> for email notifications.</p>
                            <p>This is a production-ready, secure email service.</p>
                            <p><strong>Time:</strong> " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + @"</p>
                            </body></html>"
                    );
                    result.AppendLine($"   {(testResult ? "EMAIL SENT SUCCESSFULLY" : "EMAIL SEND FAILED")}");
                    if (testResult)
                    {
                        result.AppendLine($"   Sent from: {acsSenderAddress}");
                    }
                }
                catch (Exception ex)
                {
                    result.AppendLine($"   ERROR: {ex.Message}");
                }
            }
            result.AppendLine();

            // 6. Summary
            result.AppendLine("6. SUMMARY:");
            var acsConfigured = !string.IsNullOrEmpty(acsConnectionString) && !string.IsNullOrEmpty(acsSenderAddress);
            var bloodBankExists = narasimha != null;
            var hasInventory = narasimha?.Stock != null;
            
            result.AppendLine($"   Azure Communication Services: {(acsConfigured ? "CONFIGURED" : "NOT CONFIGURED")}");
            result.AppendLine($"   Blood Bank Exists: {bloodBankExists}");
            result.AppendLine($"   Has Inventory: {hasInventory}");
            result.AppendLine();

            if (acsConfigured && bloodBankExists && hasInventory)
            {
                result.AppendLine("   ? SYSTEM READY FOR PRODUCTION!");
                result.AppendLine("   Using Azure Communication Services (secure & scalable)");
            }
            else
            {
                result.AppendLine("   ACTION REQUIRED:");
                if (!acsConfigured)
                    result.AppendLine("   - Set up Azure Communication Services (see AZURE_COMMUNICATION_SETUP.md)");
                if (!bloodBankExists)
                    result.AppendLine("   - Add Narasimha Blood Bank to database");
                if (bloodBankExists && !hasInventory)
                    result.AppendLine("   - Add blood inventory");
            }

            return Content(result.ToString(), "text/plain");
        }

        // Access: https://your-app.azurewebsites.net/Test/SendTestEmail
        public async Task<IActionResult> SendTestEmail()
        {
            try
            {
                var result = await _notificationService.SendEmailAsync(
                    to: "narasimhasez@gmail.com",
                    subject: "BloodHub Test Notification",
                    htmlBody: @"<html><body style='font-family: Arial; padding: 20px;'>
                        <h2>Test Email from BloodHub</h2>
                        <p>This is a test email to verify SMTP configuration.</p>
                        <p><strong>Time:</strong> " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + @"</p>
                        <p>If you receive this, email notifications are working!</p>
                        </body></html>"
                );

                if (result)
                {
                    return Content("Test email sent successfully to narasimhasez@gmail.com\n\nCheck your inbox and spam folder!", "text/plain");
                }
                else
                {
                    return Content("Email send failed. Check SMTP configuration.", "text/plain");
                }
            }
            catch (Exception ex)
            {
                return Content($"Error: {ex.Message}\n\n{ex.StackTrace}", "text/plain");
            }
        }
    }
}
