using System;
using System.Collections.Generic;

namespace BloodHub.Models
{
    public class DashboardViewModel
    {
        // Existing properties
        public List<Hospital> Hospitals { get; set; } = new();
        public int TotalHospitals { get; set; }
        public int TotalUnits { get; set; }
        public int LowStockHospitals { get; set; }
        public int PendingRequests { get; set; }

        // Chart totals
        public int APos { get; set; }
        public int ANeg { get; set; }
        public int BPos { get; set; }
        public int BNeg { get; set; }
        public int OPos { get; set; }
        public int ONeg { get; set; }
        public int ABPos { get; set; }
        public int ABNeg { get; set; }

        // Line chart
        public string[] Days { get; set; } = Array.Empty<string>();
        public int[] DonationsPerDay { get; set; } = Array.Empty<int>();

        // Requests / Orders lists
        public List<BloodRequest> Requests { get; set; } = new();
        public List<BloodOrder> BloodOrders { get; set; } = new();

        // Selected hospital filter
        public int? SelectedHospitalId { get; set; }

        // Active notification text (alerts)
        public string NotificationText { get; set; } = string.Empty;

        // Alerts list
        public List<Alert> Alerts { get; set; } = new();

        // ===== NEW: Donor form binding =====
        // This is the object the modal binds to: use asp-for="NewDonor.Property"
        public Donor NewDonor { get; set; } = new Donor();


        public List<Donor> Donors { get; set; } = new();
        public List<User> Users { get; set; } = new List<User>();
    }
}
