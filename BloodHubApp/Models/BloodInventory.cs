using System;

namespace BloodHub.Models
{
    public class BloodInventory
    {
        public int Id { get; set; }
        public int HospitalId { get; set; }

        // Navigation property
        public Hospital? Hospital { get; set; }

        // Blood groups
        public int APos { get; set; }
        public int ANeg { get; set; }
        public int BPos { get; set; }
        public int BNeg { get; set; }
        public int OPos { get; set; }
        public int ONeg { get; set; }
        public int ABPos { get; set; }
        public int ABNeg { get; set; }

        // Track last update
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // ✅ Computed property (not stored in DB)
        public int Total =>
            APos + ANeg + BPos + BNeg + OPos + ONeg + ABPos + ABNeg;
    }
}
