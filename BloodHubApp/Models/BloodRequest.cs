using BloodHub.Models;



public class BloodRequest
{
    public int Id { get; set; }
    public int HospitalId { get; set; }
    public string BloodGroup { get; set; }
    public int Units { get; set; }
    public string PersonName { get; set; }   // NEW
    public string MobileNumber { get; set; } // NEW
    public string Address { get; set; }      // NEW
    public string Status { get; set; } = "Pending";
    public DateTime RequestedAt { get; set; }

    public virtual Hospital Hospital { get; set; }
}
