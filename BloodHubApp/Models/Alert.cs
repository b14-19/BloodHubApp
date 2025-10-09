using System;

namespace BloodHub.Models
{
    public class Alert
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
