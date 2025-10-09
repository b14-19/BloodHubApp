using Microsoft.EntityFrameworkCore;
using BloodHub.Models;

namespace BloodHub.Data
{
    public class BloodHubContext : DbContext
    {
        public BloodHubContext(DbContextOptions<BloodHubContext> options) : base(options) { }

        public DbSet<Hospital> Hospitals => Set<Hospital>();
        public DbSet<BloodInventory> BloodInventories => Set<BloodInventory>();
        public DbSet<BloodRequest> BloodRequests => Set<BloodRequest>();
        public DbSet<BloodOrder> BloodOrders => Set<BloodOrder>();   // 🔥 Added
        public DbSet<User> Users => Set<User>();
        public DbSet<Alert> Alerts { get; set; }

        public DbSet<Donor> Donors { get; set; } = null!;



        protected override void OnModelCreating(ModelBuilder b)
        {
            b.Entity<Hospital>()
             .HasOne(h => h.Stock)
             .WithOne(i => i.Hospital!)
             .HasForeignKey<BloodInventory>(i => i.HospitalId);

            b.Entity<BloodRequest>()
             .HasOne(r => r.Hospital)
             .WithMany(h => h.Requests)
             .HasForeignKey(r => r.HospitalId);

            // 🔥 Map BloodOrders to Hospital
            b.Entity<BloodOrder>()
             .HasOne(o => o.Hospital)
             .WithMany()    // orders are not stored in Hospital.Requests (separate list)
             .HasForeignKey(o => o.HospitalId);
        }
    }
}
