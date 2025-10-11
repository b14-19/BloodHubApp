using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using BloodHub.Data;
using BloodHub.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace BloodHubApp.Controllers   // <--- change namespace if needed
{
    public class AccountController : Controller
    {
        private readonly BloodHubContext _db;

        // In-memory token store for demo only (use DB or Identity in production!)
        private static readonly ConcurrentDictionary<string, (string Email, DateTime Expiry)> _resetTokens = new();

        public AccountController(BloodHubContext db) => _db = db;

        // ---------- LOGIN ----------
        [HttpGet]
        public IActionResult Login() => View(new LoginViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _db.Users.AsNoTracking()
                                      .SingleOrDefaultAsync(u => u.Username == vm.Username);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(vm);
            }

            var suppliedPassword = vm.Password ?? string.Empty;
            var suppliedHash = ComputeSha256Hash(suppliedPassword);
            var stored = user.PasswordHash ?? string.Empty;

            var isHashMatch = string.Equals(stored, suppliedHash, StringComparison.OrdinalIgnoreCase);
            var isPlainMatch = string.Equals(stored, suppliedPassword, StringComparison.Ordinal);

            if (!isHashMatch && !isPlainMatch)
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(vm);
            }

            if (!isHashMatch && isPlainMatch)
            {
                // migrate plaintext -> hashed
                var userToUpdate = await _db.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId);
                if (userToUpdate != null)
                {
                    userToUpdate.PasswordHash = suppliedHash;
                    try { await _db.SaveChangesAsync(); }
                    catch { /* swallow/log migration error */ }
                }
            }

            // Success: create session
            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role ?? "User");



            HttpContext.Session.SetString("Username", user.Username);
            HttpContext.Session.SetString("Role", user.Role);   // "User" | "Admin" | "SuperAdmin"

            HttpContext.Session.SetString("LoginTime", DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss tt"));

            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Login));
        }

        // ---------- REGISTER ----------
        [HttpGet]
        public IActionResult Register() => View(new RegisterViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            if (await _db.Users.AnyAsync(u => u.Username == vm.Username))
            {
                ModelState.AddModelError(nameof(vm.Username), "Username already taken");
                return View(vm);
            }
            if (await _db.Users.AnyAsync(u => u.Email == vm.Email))
            {
                ModelState.AddModelError(nameof(vm.Email), "Email already registered");
                return View(vm);
            }

            var passwordHash = ComputeSha256Hash(vm.Password ?? "");

            var user = new User
            {
                Username = vm.Username,
                Email = vm.Email,
                PasswordHash = passwordHash,
                Role = "User"
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Account created. You can now log in.";
            return RedirectToAction("Login");
        }

        // ---------- FORGOT PASSWORD ----------
        [HttpGet]
        public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == vm.Email);
            if (user == null)
            {
                TempData["Info"] = "If that email exists, a reset link has been sent (dev).";
                return RedirectToAction(nameof(ForgotPassword));
            }

            var token = Guid.NewGuid().ToString("N");
            var expiry = DateTime.UtcNow.AddHours(1);
            _resetTokens[token] = (user.Email, expiry);

            var resetUrl = Url.Action(nameof(ResetPassword), "Account",
                                      new { token = token, email = user.Email }, Request.Scheme);

            // DEV: show link directly. In production, send via email.
            TempData["Info"] = $"Reset link (dev): {resetUrl}";
            return RedirectToAction(nameof(ForgotPassword));
        }

        // ---------- RESET PASSWORD ----------
        [HttpGet]
        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Invalid reset link.";
                return RedirectToAction(nameof(ForgotPassword));
            }

            if (!_resetTokens.TryGetValue(token, out var entry) ||
                !entry.Email.Equals(email, StringComparison.OrdinalIgnoreCase) ||
                entry.Expiry < DateTime.UtcNow)
            {
                TempData["Error"] = "Invalid or expired token.";
                return RedirectToAction(nameof(ForgotPassword));
            }

            return View(new ResetPasswordViewModel { Token = token, Email = email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            if (!_resetTokens.TryGetValue(vm.Token, out var entry) ||
                !entry.Email.Equals(vm.Email, StringComparison.OrdinalIgnoreCase) ||
                entry.Expiry < DateTime.UtcNow)
            {
                ModelState.AddModelError(string.Empty, "Invalid or expired token.");
                return View(vm);
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == vm.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return View(vm);
            }

            user.PasswordHash = ComputeSha256Hash(vm.Password ?? "");
            await _db.SaveChangesAsync();

            _resetTokens.TryRemove(vm.Token, out _);

            TempData["Success"] = "Password updated. You can now log in.";
            return RedirectToAction(nameof(Login));
        }

        private string ComputeSha256Hash(object value)
        {
            throw new NotImplementedException();
        }

        // ---------- HELPERS ----------
        private static string ComputeSha256Hash(string raw)
        {
            if (raw == null) raw = "";
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
            return Convert.ToHexString(bytes); // uppercase hex
        }
    }
}
