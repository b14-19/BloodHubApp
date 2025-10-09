//using System;
//using System.Linq;
//using System.Security.Cryptography;
//using System.Text;
//using System.Threading.Tasks;
//using BloodHub.Data;
//using BloodHub.Models;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;

//namespace BloodHub.Controllers
//{
//    // AuthController provides registration and password-reset flows without touching your existing AccountController.
//    public class AuthController : Controller
//    {
//        private readonly BloodHubContext _db;

//        // dev-only in-memory token store for forgot/reset password (replace with DB or persistent store in production)
//        private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, (string Email, DateTime Expiry)> _resetTokens =
//            new();

//        public AuthController(BloodHubContext db) => _db = db;

//        // -------------------------
//        // REGISTER
//        // GET: /Auth/Register
//        // -------------------------
//        [HttpGet]
//        public IActionResult Register() => View(new RegisterViewModel());

//        // POST: /Auth/Register
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Register(RegisterViewModel vm)
//        {
//            if (!ModelState.IsValid) return View(vm);

//            // uniqueness checks
//            if (await _db.Users.AnyAsync(u => u.Username == vm.Username))
//            {
//                ModelState.AddModelError(nameof(vm.Username), "Username already taken");
//                return View(vm);
//            }
//            if (!string.IsNullOrEmpty(vm.Email) && await _db.Users.AnyAsync(u => u.Email == vm.Email))
//            {
//                ModelState.AddModelError(nameof(vm.Email), "Email already registered");
//                return View(vm);
//            }

//            // hash password (dev example). Replace with proper hashing in production.
//            var passwordHash = ComputeSha256Hash(vm.Password);

//            var user = new User
//            {
//                Username = vm.Username,
//                Email = string.IsNullOrWhiteSpace(vm.Email) ? null : vm.Email,
//                PasswordHash = passwordHash,
//                Role = "User"
//            };

//            _db.Users.Add(user);
//            await _db.SaveChangesAsync();

//            TempData["Success"] = "Account created. You can now log in.";
//            // redirect to whichever login you prefer (Auth or Account)
//            return RedirectToAction("Login", "Account");
//        }

//        // -------------------------
//        // FORGOT PASSWORD
//        // GET: /Auth/ForgotPassword
//        // -------------------------
//        [HttpGet]
//        public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());

//        // POST: /Auth/ForgotPassword
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel vm)
//        {
//            if (!ModelState.IsValid) return View(vm);

//            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == vm.Email);
//            if (user == null)
//            {
//                // generic message so we don't reveal whether email exists
//                TempData["Info"] = "If that email exists, a reset link has been sent.";
//                return RedirectToAction(nameof(ForgotPassword));
//            }

//            // create token and persist it to in-memory store (dev). In prod save in DB and send email.
//            var token = Guid.NewGuid().ToString("N");
//            var expiry = DateTime.UtcNow.AddHours(1);
//            _resetTokens[token] = (user.Email!, expiry);

//            // Build reset URL
//            var resetUrl = Url.Action(nameof(ResetPassword), "Auth", new { token, email = user.Email }, Request.Scheme);

//            // For dev: show the reset link in TempData so you can copy it from UI.
//            TempData["Info"] = $"Reset link (dev): {resetUrl}";

//            // In production, send resetUrl via email to user.Email
//            return RedirectToAction(nameof(ForgotPassword));
//        }

//        // -------------------------
//        // RESET PASSWORD
//        // GET: /Auth/ResetPassword
//        // -------------------------
//        [HttpGet]
//        public IActionResult ResetPassword(string token, string email)
//        {
//            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email)) return BadRequest();
//            var vm = new ResetPasswordViewModel { Token = token, Email = email };
//            return View(vm);
//        }

//        // POST: /Auth/ResetPassword
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
//        {
//            if (!ModelState.IsValid) return View(vm);

//            if (!_resetTokens.TryGetValue(vm.Token, out var info) || info.Email != vm.Email || info.Expiry < DateTime.UtcNow)
//            {
//                ModelState.AddModelError(string.Empty, "Invalid or expired token");
//                return View(vm);
//            }

//            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == vm.Email);
//            if (user == null)
//            {
//                ModelState.AddModelError(string.Empty, "User not found");
//                return View(vm);
//            }

//            user.PasswordHash = ComputeSha256Hash(vm.NewPassword);
//            await _db.SaveChangesAsync();

//            // remove token from memory store
//            _resetTokens.TryRemove(vm.Token, out _);

//            TempData["Success"] = "Password updated. You can now log in.";
//            // redirect to whichever login page you wish (Account or Auth)
//            return RedirectToAction("Login", "Account");
//        }

//        // -------------------------
//        // OPTIONAL: A Login GET for Auth (can reuse Account.Login instead)
//        // GET: /Auth/Login
//        // -------------------------
//        [HttpGet]
//        public IActionResult Login() => View(new LoginViewModel());

//        // -------------------------
//        // Helper: SHA256 (dev)
//        // -------------------------
//        private static string ComputeSha256Hash(string raw)
//        {
//            using var sha = SHA256.Create();
//            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(raw ?? ""));
//            return Convert.ToHexString(bytes);
//        }
//    }
//}
