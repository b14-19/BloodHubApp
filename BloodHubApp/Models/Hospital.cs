using BloodHub.Models;  // 👈 Add this

namespace BloodHub.Models;
public class Hospital
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string City { get; set; } = "";
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public BloodInventory? Stock { get; set; }
    public ICollection<BloodRequest> Requests { get; set; } = new List<BloodRequest>();
}
