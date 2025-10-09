using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BloodHub.Data;
using BloodHub.Models;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace BloodHub.Controllers
{
    public class RequestsController : Controller
    {
        private readonly BloodHubContext _db;
        public RequestsController(BloodHubContext db) => _db = db;

        // Show all requests
        public async Task<IActionResult> Index()
        {
            var requests = await _db.BloodRequests
                .Include(r => r.Hospital)
                .OrderByDescending(r => r.RequestedAt)
                .ToListAsync();

            return View(requests);
        }

        // Create GET
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Hospitals = _db.Hospitals.ToList();
            return View();
        }

        // Create POST
        [HttpPost]
        public async Task<IActionResult> Create(BloodRequest req)
        {
            if (ModelState.IsValid)
            {
                req.Status = "Pending";
                req.RequestedAt = DateTime.Now;

                _db.BloodRequests.Add(req);
                await _db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            ViewBag.Hospitals = _db.Hospitals.ToList();
            return View(req);
        }
    }
}
