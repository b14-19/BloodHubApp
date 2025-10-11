using System;
using System.ComponentModel.DataAnnotations;

namespace BloodHub.Models
{
    public class Donor
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string DonorName { get; set; } = "";

        [Required, StringLength(20)]
        public string MobileNumber { get; set; } = "";

        [Required, StringLength(500)]
        public string Address { get; set; } = "";

        // Use nullable int if you want Required to be detected properly by model binder,
        // or keep int + Range(1,...) to require a valid selection (>0).
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a hospital.")]
        public int HospitalId { get; set; }

        [Required]
        public string ServiceType { get; set; } = "Free"; // "Free" or "Paid"

        [Required, StringLength(20)]
        public string Gender { get; set; } = "Other"; // "Male", "Female", "Other"

        [Required, StringLength(200)]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required, StringLength(10)]
        public string BloodGroup { get; set; } = ""; // A+, A-, B+, B-, O+, O-, AB+, AB-

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // navigation
        public Hospital? Hospital { get; set; }
    }
}
