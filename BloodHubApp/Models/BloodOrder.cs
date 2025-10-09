using System;

namespace BloodHub.Models
{
    public class BloodOrder
    {
        public int Id { get; set; }
        public int HospitalId { get; set; }
        public string BloodGroup { get; set; } = string.Empty;
        public int Units { get; set; }
        public string Status { get; set; } = "Pending";
        public DateTime OrderedAt { get; set; } = DateTime.Now;

        public Hospital Hospital { get; set; } = null!;
    }
}
