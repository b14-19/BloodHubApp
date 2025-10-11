using BloodHub.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using BloodHub.Models;
using System.Security.Cryptography;
using System.Text;
using BloodHubApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Resolve connection string with Azure fallbacks
string? conn =
    builder.Configuration.GetConnectionString("BloodHub")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings:BloodHub")
    ?? Environment.GetEnvironmentVariable("SQLCONNSTR_BloodHub")
    ?? Environment.GetEnvironmentVariable("SQLAZURECONNSTR_BloodHub")
    ?? Environment.GetEnvironmentVariable("CUSTOMCONNSTR_BloodHub");

if (string.IsNullOrWhiteSpace(conn))
{
    throw new InvalidOperationException("Connection string 'BloodHub' not found. Add it in App Service > Environment variables > Connection strings, then restart.");
}

// Allow more time for Serverless DB cold start
var csb = new SqlConnectionStringBuilder(conn);
if (csb.ConnectTimeout < 60) csb.ConnectTimeout = 60;
conn = csb.ConnectionString;

// MVC + DbContext + Session
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<BloodHubContext>(o =>
    o.UseSqlServer(
        conn,
        sql =>
        {
            sql.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), errorNumbersToAdd: null);
            sql.CommandTimeout(60);
        }));
builder.Services.AddSession();

// Register NotificationService - Use Azure Communication Services for production
builder.Services.AddScoped<INotificationService, AzureEmailService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseSession();

// Helpful diagnostics
app.Logger.LogInformation("Environment: {Env}", app.Environment.EnvironmentName);
app.Logger.LogInformation("ConnectionStrings:BloodHub present: {HasConn}", !string.IsNullOrWhiteSpace(conn));

// Apply EF Core migrations with a 60s cap to avoid 500.37 during cold start
await InitializeDatabaseAsync(app.Services, app.Logger, TimeSpan.FromSeconds(60));

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();

static async Task InitializeDatabaseAsync(IServiceProvider services, ILogger logger, TimeSpan timeout)
{
    using var scope = services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<BloodHubContext>();

    try
    {
        using var cts = new CancellationTokenSource(timeout);

        if (await db.Database.CanConnectAsync(cts.Token))
        {
            logger.LogInformation("DB reachable. Applying EF Core migrations...");
            await db.Database.MigrateAsync(cts.Token);
            logger.LogInformation("Migrations applied successfully.");

            await SeedDefaultAdminAsync(db, logger, cts.Token);
        }
        else
        {
            logger.LogError("DB not reachable within {Timeout}s. Skipping migrations to allow app startup.", timeout.TotalSeconds);
        }
    }
    catch (OperationCanceledException)
    {
        logger.LogError("DB initialization timed out after {Timeout}s. Skipping migrations.", timeout.TotalSeconds);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "DB initialization failed. Skipping migrations to allow app startup.");
    }
}

static async Task SeedDefaultAdminAsync(BloodHubContext db, ILogger logger, CancellationToken ct)
{
    try
    {
        var adminExists = await db.Users.AnyAsync(u => u.Username == "admin", ct);
        if (!adminExists)
        {
            var pwd = Environment.GetEnvironmentVariable("DEFAULT_ADMIN_PASSWORD") ?? "Admin@123";
            var hash = ComputeSha256(pwd);

            db.Users.Add(new User
            {
                Username = "admin",
                Email = "admin@bloodhub.local",
                PasswordHash = hash,
                Role = "Admin"
            });
            await db.SaveChangesAsync(ct);
            logger.LogInformation("Seeded default admin user 'admin'.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to seed default admin user.");
    }
}

static string ComputeSha256(string raw)
{
    raw ??= "";
    using var sha = SHA256.Create();
    var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
    return Convert.ToHexString(bytes);
}