// Models/AccountViewModels.cs
using System.ComponentModel.DataAnnotations;

namespace BloodHub.Models
{
    public class RegisterViewModel
    {
        [Required, StringLength(100)]
        public string Username { get; set; }

        [Required, EmailAddress, StringLength(200)]
        public string Email { get; set; }

        [Required, StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required, StringLength(100)]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }

    public class ForgotPasswordViewModel
    {
        [Required, EmailAddress]
        public string Email { get; set; }
    }

    public class ResetPasswordViewModel
    {
        [Required] public string Token { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        [Required, DataType(DataType.Password)] public string NewPassword { get; set; }
        [DataType(DataType.Password), Compare("NewPassword")] public string ConfirmPassword { get; set; }
        public string? Password { get; internal set; }
    }
}
