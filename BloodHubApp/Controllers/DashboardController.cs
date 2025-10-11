using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BloodHub.Data;
using BloodHub.Models;
using BloodHub.Models.BloodHub.Models;
using Azure.Core;
using System.Diagnostics;
using System.Text.Json;
using System.Text;
using System.Security.Cryptography;
using BloodHubApp.Services;

namespace BloodHub.Controllers
{
    public class DashboardController : Controller
    {
        private readonly BloodHubContext _db;
        private readonly INotificationService _notificationService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            BloodHubContext db, 
            INotificationService notificationService,
            ILogger<DashboardController> logger)
        {
            _db = db;
            _notificationService = notificationService;
            _logger = logger;
        }

        [HttpGet]



        //public async Task<IActionResult> Index(string? mode, int? hospitalId)
        //{
        //    if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
        //        return RedirectToAction("Login", "Account");

        //    // Fetch hospitals (filtered if hospitalId provided)
        //    var hospitalsQuery = _db.Hospitals
        //        .Include(h => h.Stock)
        //        .Include(h => h.Requests)
        //        .AsQueryable();

        //    if (hospitalId.HasValue)
        //        hospitalsQuery = hospitalsQuery.Where(h => h.Id == hospitalId);

        //    var hospitals = await hospitalsQuery
        //        .OrderBy(h => h.City).ThenBy(h => h.Name)
        //        .ToListAsync();

        //    // KPIs (use all hospitals, not filtered ones)
        //    var allHospitals = await _db.Hospitals.Include(h => h.Stock).ToListAsync();

        //    int totalUnits = allHospitals.Sum(h => h.Stock?.Total ?? 0);
        //    int lowStockHospitals = allHospitals.Count(h => h.Stock != null &&
        //        (h.Stock.APos <= 10 || h.Stock.ANeg <= 10 || h.Stock.BPos <= 10 || h.Stock.BNeg <= 10 ||
        //         h.Stock.OPos <= 10 || h.Stock.ONeg <= 10 || h.Stock.ABPos <= 10 || h.Stock.ABNeg <= 10));
        //    int pendingRequests = await _db.BloodRequests.CountAsync(r => r.Status == "Pending");

        //    // Totals for bar chart
        //    int aPos = allHospitals.Sum(h => h.Stock?.APos ?? 0);
        //    int aNeg = allHospitals.Sum(h => h.Stock?.ANeg ?? 0);
        //    int bPos = allHospitals.Sum(h => h.Stock?.BPos ?? 0);
        //    int bNeg = allHospitals.Sum(h => h.Stock?.BNeg ?? 0);
        //    int oPos = allHospitals.Sum(h => h.Stock?.OPos ?? 0);
        //    int oNeg = allHospitals.Sum(h => h.Stock?.ONeg ?? 0);
        //    int abPos = allHospitals.Sum(h => h.Stock?.ABPos ?? 0);
        //    int abNeg = allHospitals.Sum(h => h.Stock?.ABNeg ?? 0);

        //    // Line chart (last 7 days)
        //    var dayList = Enumerable.Range(0, 7)
        //        .Select(i => DateTime.Today.AddDays(-6 + i))
        //        .ToArray();

        //    var donationsPerDay = await _db.BloodRequests
        //        .Where(r => r.RequestedAt >= dayList.First() && r.RequestedAt < dayList.Last().AddDays(1))
        //        .GroupBy(r => r.RequestedAt.Date)
        //        .Select(g => new { Day = g.Key, Count = g.Count() })
        //        .ToListAsync();

        //    int[] perDay = dayList
        //        .Select(d => donationsPerDay.FirstOrDefault(x => x.Day == d)?.Count ?? 0)
        //        .ToArray();

        //    // Requests + Orders (filtered if hospitalId is provided)
        //    var requests = await _db.BloodRequests
        //        .Include(r => r.Hospital)
        //        .Where(r => !hospitalId.HasValue || r.HospitalId == hospitalId)
        //        .OrderByDescending(r => r.RequestedAt)
        //        .ToListAsync();

        //    var orders = await _db.BloodOrders
        //        .Include(o => o.Hospital)
        //        .Where(o => !hospitalId.HasValue || o.HospitalId == hospitalId)
        //        .OrderByDescending(o => o.OrderedAt)
        //        .ToListAsync();

        //    // Active + recent alerts
        //    var activeAlert = await _db.Alerts
        //        .Where(a => a.IsActive)
        //        .OrderByDescending(a => a.CreatedAt)
        //        .FirstOrDefaultAsync();

        //    var recentAlerts = await _db.Alerts
        //        .OrderByDescending(a => a.CreatedAt)
        //        .ToListAsync();

        //    // Load users for the users view/table (unfiltered)
        //    var users = await _db.Users
        //        .OrderBy(u => u.Username)
        //        .ToListAsync();

        //    var vm = new DashboardViewModel
        //    {
        //        Hospitals = hospitals,
        //        TotalHospitals = allHospitals.Count,
        //        TotalUnits = totalUnits,
        //        LowStockHospitals = lowStockHospitals,
        //        PendingRequests = pendingRequests,

        //        APos = aPos,
        //        ANeg = aNeg,
        //        BPos = bPos,
        //        BNeg = bNeg,
        //        OPos = oPos,
        //        ONeg = oNeg,
        //        ABPos = abPos,
        //        ABNeg = abNeg,

        //        Days = dayList.Select(d => d.ToString("dd MMM")).ToArray(),
        //        DonationsPerDay = perDay,

        //        Requests = requests,
        //        BloodOrders = orders,

        //        SelectedHospitalId = hospitalId,

        //        NotificationText = activeAlert?.Text ?? string.Empty,
        //        Alerts = recentAlerts,

        //        // new: include users so Model.Users is available in the view
        //        Users = users
        //    };

        //    // PASS unfiltered hospitals for modal dropdown (server-side options)
        //    ViewBag.Hospitals = await _db.Hospitals.OrderBy(h => h.Name).ToListAsync();

        //    // If DonorService POST set TempData["DonorFeedback"], move it to ViewBag for client script
        //    if (TempData.ContainsKey("DonorFeedback"))
        //    {
        //        var donorFeedbackJson = TempData["DonorFeedback"] as string;
        //        if (!string.IsNullOrEmpty(donorFeedbackJson))
        //            ViewBag.DonorFeedbackJson = donorFeedbackJson;
        //    }

        //    // Optional: show Success / Error from TempData as global message on index
        //    if (TempData.ContainsKey("Success"))
        //        ViewBag.GlobalSuccess = TempData["Success"]?.ToString();
        //    if (TempData.ContainsKey("Error"))
        //        ViewBag.GlobalError = TempData["Error"]?.ToString();



        //    ViewBag.Mode = mode;
        //    return View(vm);
        //}
        [HttpGet]
        public async Task<IActionResult> Index(string? mode, int? hospitalId, int? page)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToAction("Login", "Account");

            // Fetch hospitals (filtered if hospitalId provided)
            var hospitalsQuery = _db.Hospitals
                .Include(h => h.Stock)
                .Include(h => h.Requests)
                .AsQueryable();

            if (hospitalId.HasValue)
                hospitalsQuery = hospitalsQuery.Where(h => h.Id == hospitalId);

            var hospitals = await hospitalsQuery
                .OrderBy(h => h.City).ThenBy(h => h.Name)
                .ToListAsync();

            // KPIs (use all hospitals, not filtered ones)
            var allHospitals = await _db.Hospitals.Include(h => h.Stock).ToListAsync();

            int totalUnits = allHospitals.Sum(h => h.Stock?.Total ?? 0);
            int lowStockHospitals = allHospitals.Count(h => h.Stock != null &&
                (h.Stock.APos <= 10 || h.Stock.ANeg <= 10 || h.Stock.BPos <= 10 || h.Stock.BNeg <= 10 ||
                 h.Stock.OPos <= 10 || h.Stock.ONeg <= 10 || h.Stock.ABPos <= 10 || h.Stock.ABNeg <= 10));
            int pendingRequests = await _db.BloodRequests.CountAsync(r => r.Status == "Pending");

            // Totals for bar chart
            int aPos = allHospitals.Sum(h => h.Stock?.APos ?? 0);
            int aNeg = allHospitals.Sum(h => h.Stock?.ANeg ?? 0);
            int bPos = allHospitals.Sum(h => h.Stock?.BPos ?? 0);
            int bNeg = allHospitals.Sum(h => h.Stock?.BNeg ?? 0);
            int oPos = allHospitals.Sum(h => h.Stock?.OPos ?? 0);
            int oNeg = allHospitals.Sum(h => h.Stock?.ONeg ?? 0);
            int abPos = allHospitals.Sum(h => h.Stock?.ABPos ?? 0);
            int abNeg = allHospitals.Sum(h => h.Stock?.ABNeg ?? 0);

            // Line chart (last 7 days)
            var dayList = Enumerable.Range(0, 7)
                .Select(i => DateTime.Today.AddDays(-6 + i))
                .ToArray();

            var donationsPerDay = await _db.BloodRequests
                .Where(r => r.RequestedAt >= dayList.First() && r.RequestedAt < dayList.Last().AddDays(1))
                .GroupBy(r => r.RequestedAt.Date)
                .Select(g => new { Day = g.Key, Count = g.Count() })
                .ToListAsync();

            int[] perDay = dayList
                .Select(d => donationsPerDay.FirstOrDefault(x => x.Day == d)?.Count ?? 0)
                .ToArray();

            // Requests + Orders (filtered if hospitalId is provided)
            var requests = await _db.BloodRequests
                .Include(r => r.Hospital)
                .Where(r => !hospitalId.HasValue || r.HospitalId == hospitalId)
                .OrderByDescending(r => r.RequestedAt)
                .ToListAsync();

            var orders = await _db.BloodOrders
                .Include(o => o.Hospital)
                .Where(o => !hospitalId.HasValue || o.HospitalId == hospitalId)
                .OrderByDescending(o => o.OrderedAt)
                .ToListAsync();

            // Active + recent alerts
            var activeAlert = await _db.Alerts
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefaultAsync();

            var recentAlerts = await _db.Alerts
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            // Load users for the users view/table (unfiltered)
            var users = await _db.Users
                .OrderBy(u => u.Username)
                .ToListAsync();

            // ----- Donors with server-side paging (20 per page) -----
            const int donorPageSize = 20;
            int donorPage = page.GetValueOrDefault(1);
            if (donorPage < 1) donorPage = 1;

            List<Donor> donors = new List<Donor>();
            int donorsTotalCount = 0;

            if (string.Equals(mode, "donordata", StringComparison.OrdinalIgnoreCase) && hospitalId.HasValue)
            {
                // total count for pagination
                donorsTotalCount = await _db.Donors
                    .Where(d => d.HospitalId == hospitalId)
                    .CountAsync();

                // fetch single page and include Hospital navigation
                donors = await _db.Donors
                    .Include(d => d.Hospital)
                    .Where(d => d.HospitalId == hospitalId)
                    .OrderByDescending(d => d.CreatedAt)
                    .Skip((donorPage - 1) * donorPageSize)
                    .Take(donorPageSize)
                    .ToListAsync();
            }

            // Build the view model (single vm with donors included)
            var vm = new DashboardViewModel
            {
                Hospitals = hospitals,
                TotalHospitals = allHospitals.Count,
                TotalUnits = totalUnits,
                LowStockHospitals = lowStockHospitals,
                PendingRequests = pendingRequests,

                APos = aPos,
                ANeg = aNeg,
                BPos = bPos,
                BNeg = bNeg,
                OPos = oPos,
                ONeg = oNeg,
                ABPos = abPos,
                ABNeg = abNeg,

                Days = dayList.Select(d => d.ToString("dd MMM")).ToArray(),
                DonationsPerDay = perDay,

                Requests = requests,
                BloodOrders = orders,

                SelectedHospitalId = hospitalId,

                NotificationText = activeAlert?.Text ?? string.Empty,
                Alerts = recentAlerts,

                Users = users,
                Donors = donors   // <--- donors included here
            };

            // PASS unfiltered hospitals for modal dropdown (server-side options)
            ViewBag.Hospitals = await _db.Hospitals.OrderBy(h => h.Name).ToListAsync();

            // If DonorService POST set TempData["DonorFeedback"], move it to ViewBag for client script
            if (TempData.ContainsKey("DonorFeedback"))
            {
                var donorFeedbackJson = TempData["DonorFeedback"] as string;
                if (!string.IsNullOrEmpty(donorFeedbackJson))
                    ViewBag.DonorFeedbackJson = donorFeedbackJson;
            }

            // Optional: show Success / Error from TempData as global message on index
            if (TempData.ContainsKey("Success"))
                ViewBag.GlobalSuccess = TempData["Success"]?.ToString();
            if (TempData.ContainsKey("Error"))
                ViewBag.GlobalError = TempData["Error"]?.ToString();

            // Pass paging info to view
            ViewBag.DonorPageSize = donorPageSize;
            ViewBag.DonorPage = donorPage;
            ViewBag.DonorTotalCount = donorsTotalCount;
            ViewBag.DonorTotalPages = (int)Math.Ceiling(donorsTotalCount / (double)donorPageSize);

            ViewBag.Mode = mode;
            return View(vm);
        }


        [HttpPost]
        //public async Task<IActionResult> SaveRequest(int hospitalId, string bloodGroup, int units, string personName, string mobileNumber, string address)
        //{
        //    if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
        //        return RedirectToAction("Login", "Account");

        //    var req = new BloodRequest
        //    {
        //        HospitalId = hospitalId,
        //        BloodGroup = bloodGroup,
        //        Units = units,
        //        PersonName = personName,       // NEW
        //        MobileNumber = mobileNumber,   // NEW
        //        Address = address,             // NEW
        //        Status = "Pending",
        //        RequestedAt = DateTime.Now
        //    };

        //    _db.BloodRequests.Add(req);
        //    await _db.SaveChangesAsync();

        //    TempData["Success"] = "Request  updated successfully ✅";

        //    return RedirectToAction("Index", new { mode = "requests" });
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveRequest(
            int hospitalId, string bloodGroup, int units,
            string personName, string mobileNumber, string address)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToAction("Login", "Account");

            var req = new BloodRequest
            {
                HospitalId = hospitalId,
                BloodGroup = bloodGroup,
                Units = units,
                PersonName = personName,
                MobileNumber = mobileNumber,
                Address = address,
                Status = "Pending",
                RequestedAt = DateTime.Now
            };

            _db.BloodRequests.Add(req);
            await _db.SaveChangesAsync();

            // Get requesting hospital details (from Users table - the actual hospital)
            var requestingUsername = HttpContext.Session.GetString("Username");
            var requestingHospital = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == requestingUsername);

            var requestingHospitalName = requestingHospital?.Username ?? "Hospital";
            var requestingHospitalEmail = requestingHospital?.Email ?? "";

            // Send notifications to blood banks (Hospitals table) that have sufficient stock
            try
            {
                // Find blood banks with sufficient stock for the requested blood group
                var bloodBanks = await _db.Hospitals
                    .Include(h => h.Stock)
                    .Where(h => h.IsActive && 
                                h.Email != null && 
                                h.Stock != null)
                    .ToListAsync();

                // Filter blood banks that have sufficient units
                var matchingBloodBanks = bloodBanks.Where(bb =>
                {
                    if (bb.Stock == null) return false;

                    return bloodGroup switch
                    {
                        "A+" => bb.Stock.APos >= units,
                        "A-" => bb.Stock.ANeg >= units,
                        "B+" => bb.Stock.BPos >= units,
                        "B-" => bb.Stock.BNeg >= units,
                        "O+" => bb.Stock.OPos >= units,
                        "O-" => bb.Stock.ONeg >= units,
                        "AB+" => bb.Stock.ABPos >= units,
                        "AB-" => bb.Stock.ABNeg >= units,
                        _ => false
                    };
                }).ToList();

                _logger.LogInformation(
                    "Blood request created by {Hospital}: {BloodGroup}, {Units} units. Found {Count} matching blood banks",
                    requestingHospitalName, bloodGroup, units, matchingBloodBanks.Count);

                if (matchingBloodBanks.Any())
                {
                    var emailSubject = $"🩸 Blood Request: {bloodGroup} ({units} units) - From {requestingHospitalName}";
                    var emailBody = $@"
                        <html>
                        <body style='font-family: Arial, sans-serif; padding: 20px;'>
                            <h2 style='color: #d32f2f;'>New Blood Request from Hospital</h2>
                            <div style='background: #f5f5f5; padding: 15px; border-left: 4px solid #d32f2f;'>
                                <h3>Requesting Hospital Details:</h3>
                                <p><strong>Hospital Name:</strong> {requestingHospitalName}</p>
                                <p><strong>Contact Email:</strong> {requestingHospitalEmail}</p>
                                <p><strong>Contact Phone:</strong> {mobileNumber}</p>
                            </div>
                            <div style='margin-top: 20px;'>
                                <h3>Blood Requirement:</h3>
                                <p><strong>Blood Group:</strong> <span style='color: #d32f2f; font-weight: bold; font-size: 18px;'>{bloodGroup}</span></p>
                                <p><strong>Units Required:</strong> {units}</p>
                                <p><strong>Patient Name:</strong> {personName}</p>
                                <p><strong>Patient Address:</strong> {address}</p>
                                <p><strong>Requested On:</strong> {DateTime.Now:dd MMM yyyy, hh:mm tt}</p>
                            </div>
                            <hr/>
                            <p style='color: #555;'>Please contact the requesting hospital if you can fulfill this blood requirement.</p>
                            <p style='color: #999; font-size: 12px;'>This is an automated notification from BloodHub System.</p>
                        </body>
                        </html>
                    ";

                    // Send emails to all matching blood banks
                    var emailTasks = matchingBloodBanks.Select(async bloodBank =>
                    {
                        try
                        {
                            await _notificationService.SendEmailAsync(
                                to: bloodBank.Email!,
                                subject: emailSubject,
                                htmlBody: emailBody
                            );
                            _logger.LogInformation("Notification sent to blood bank: {BloodBank} ({Email})", 
                                bloodBank.Name, bloodBank.Email);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to send notification to blood bank {BloodBank}", bloodBank.Name);
                        }
                    });

                    await Task.WhenAll(emailTasks);
                }
                else
                {
                    _logger.LogInformation("No blood banks found with sufficient {BloodGroup} stock ({Units} units)", 
                        bloodGroup, units);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending blood request notifications");
            }

            // Prepare PDF data
            var dto = new
            {
                id = req.Id,
                hospitalId = req.HospitalId,
                hospitalName = requestingHospitalName,
                bloodGroup = req.BloodGroup,
                units = req.Units,
                personName = req.PersonName,
                mobileNumber = req.MobileNumber,
                address = req.Address,
                status = req.Status,
                requestedAt = req.RequestedAt
            };

            TempData["LastRequestJson"] = System.Text.Json.JsonSerializer.Serialize(
                dto,
                new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = null,
                    WriteIndented = false
                });

            TempData["Success"] = "Request created successfully ✅";
            return RedirectToAction("Index", new { mode = "requests" });
        }

        public IActionResult Index1(string mode)
        {
            var model = new DashboardViewModel
            {
                NotificationText = "🩸 Urgent Requirement: O+ blood needed at City Care Hospital. Please donate if eligible."
            };

            return View(model);
        }










        [HttpGet]
        public async Task<IActionResult> SearchHospital(int id)
        {
            var hospital = await _db.Hospitals
                .Include(h => h.Stock)
                .Include(h => h.Requests)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hospital == null)
            {
                TempData["Error"] = "Hospital not found!";
                return RedirectToAction("Index", new { mode = "hospitals" });
            }

            var vm = new DashboardViewModel
            {
                Hospitals = new List<Hospital> { hospital },
                Requests = hospital.Requests.ToList()
            };

            return View("Index", vm);
        }









        [HttpPost]
        public async Task<IActionResult> UpdateHospital(Hospital hospital)
        {
            var existingHospital = await _db.Hospitals
                .Include(h => h.Stock)
                .FirstOrDefaultAsync(h => h.Id == hospital.Id);

            if (existingHospital == null)
                return NotFound();

            // Update hospital details
            existingHospital.Name = hospital.Name;
            existingHospital.Email = hospital.Email;
            existingHospital.Phone = hospital.Phone;
            existingHospital.City = hospital.City;

            // Update stock
            if (existingHospital.Stock != null && hospital.Stock != null)
            {
                existingHospital.Stock.APos = hospital.Stock.APos;
                existingHospital.Stock.ANeg = hospital.Stock.ANeg;
                existingHospital.Stock.BPos = hospital.Stock.BPos;
                existingHospital.Stock.BNeg = hospital.Stock.BNeg;
                existingHospital.Stock.OPos = hospital.Stock.OPos;
                existingHospital.Stock.ONeg = hospital.Stock.ONeg;
                existingHospital.Stock.ABPos = hospital.Stock.ABPos;
                existingHospital.Stock.ABNeg = hospital.Stock.ABNeg;
                existingHospital.Stock.UpdatedAt = DateTime.Now;  // ✅ track last update
            }

            await _db.SaveChangesAsync();

            TempData["Success"] = "Hospital details updated successfully ✅";
            return RedirectToAction("Index", new { mode = "hospitals", hospitalId = hospital.Id });
        }









        // GET: /Dashboard/AlertsTextEnter
        [HttpGet]
        public async Task<IActionResult> AlertsTextEnter()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToAction("Login", "Account");

            // load currently active alert to prefill editor (if any)
            var active = await _db.Alerts
                .Where(a => a.IsActive)
                .OrderByDescending(a => a.CreatedAt)
                .FirstOrDefaultAsync();

            var vm = new AlertsTextViewModel
            {
                Text = active?.Text ?? string.Empty
            };
            return View(vm);
        }

        // POST: /Dashboard/AlertsTextEnter
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlertsTextEnter(AlertsTextViewModel model)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please enter alert text.";
                return View(model);
            }

            try
            {
                // Deactivate existing active alerts
                var activeAlerts = await _db.Alerts.Where(a => a.IsActive).ToListAsync();
                foreach (var a in activeAlerts)
                    a.IsActive = false;

                var alert = new Alert
                {
                    Text = model.Text,
                    CreatedBy = HttpContext.Session.GetString("Username"),
                    CreatedAt = DateTime.Now,   // use local time to match SSMS display; change to UtcNow if desired
                    IsActive = true
                };

                _db.Alerts.Add(alert);
                await _db.SaveChangesAsync();

                TempData["Success"] = "Alert saved.";
                // redirect to the alerts view so user sees the list + active alert immediately
                return RedirectToAction("Index", new { mode = "alerts" });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error saving alert: " + ex.Message;
                return View(model);
            }



        }




        // Optional GET view if someone hits /Dashboard/DonorService directly — keep for compatibility
        [HttpGet]
        public async Task<IActionResult> DonorService()
        {
            ViewBag.Hospitals = await _db.Hospitals.ToListAsync();
            return RedirectToAction("Index", new { mode = "donor" }); // redirect into Index modal
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> DonorService(Donor donor)
        {
            // ensure hospitals are available if we re-render view (we actually redirect to Index)
            if (!ModelState.IsValid)
            {
                // collect model errors into simple array
                var errors = ModelState
                    .Where(kvp => kvp.Value.Errors.Any())
                    .SelectMany(kvp => kvp.Value.Errors.Select(e => new { Field = kvp.Key, Error = e.ErrorMessage }))
                    .ToList();

                // build payload we can store in TempData (string)
                var feedback = new
                {
                    success = false,
                    errors = errors,
                    formData = new
                    {
                        donor.DonorName,
                        donor.Gender,
                        donor.MobileNumber,
                        donor.Email,
                        donor.Address,
                        donor.HospitalId,
                        donor.ServiceType
                    }
                };

                TempData["DonorFeedback"] = JsonSerializer.Serialize(feedback);

                // redirect back to Index page with mode=donor so Index shows the modal and JS can re-populate
                return RedirectToAction("Index", new { mode = "donor" });
            }

            // save donor
            _db.Donors.Add(donor);
            await _db.SaveChangesAsync();

            // on success, set simple success message and optionally a success payload
            var successPayload = new
            {
                success = true,
                message = "Donor details saved successfully!"
            };
            TempData["Success"] = successPayload.message;
            TempData["DonorFeedback"] = JsonSerializer.Serialize(successPayload);

            return RedirectToAction("Index", new { mode = "full" });
        }









        // POST - Create new user (SaveUser)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveUser([Bind("Username,PasswordHash,Role,Email")] User user)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(user.Username) || string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                TempData["Error"] = "Username and password are required.";
                return RedirectToAction("Index", new { mode = "users" });
            }

            // Hash password (demo: SHA256). Replace with BCrypt/Identity for production.
            using (var sha = SHA256.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(user.PasswordHash));
                user.PasswordHash = string.Concat(bytes.Select(b => b.ToString("x2")));
            }

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            TempData["Success"] = "User created successfully.";
            return RedirectToAction("Index", new { mode = "users" });
        }

        // GET - return user JSON for modal prefill
        [HttpGet]
        public async Task<IActionResult> GetUserJson(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return Unauthorized();

            var u = await _db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == id);
            if (u == null) return NotFound();

            // don't return password hash
            var outObj = new
            {
                u.UserId,
                u.Username,
                u.Role,
                u.Email
            };
            return Json(outObj);
        }

        // POST - update existing user (modal submits here)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser([Bind("UserId,Username,PasswordHash,Role,Email")] User input)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToAction("Login", "Account");

            var existing = await _db.Users.FindAsync(input.UserId);
            if (existing == null) return NotFound();

            existing.Username = input.Username;
            existing.Role = input.Role;
            existing.Email = input.Email;

            // if password field was filled, hash and update
            if (!string.IsNullOrWhiteSpace(input.PasswordHash))
            {
                using var sha = SHA256.Create();
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input.PasswordHash));
                existing.PasswordHash = string.Concat(bytes.Select(b => b.ToString("x2")));
            }

            await _db.SaveChangesAsync();
            TempData["Success"] = "User updated successfully.";
            return RedirectToAction("Index", new { mode = "users" });
        }

        // POST - delete user (safe POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToAction("Login", "Account");

            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            TempData["Success"] = "User deleted.";
            return RedirectToAction("Index", new { mode = "users" });
        }




        // GET - return request as JSON for modal prefill
        [HttpGet]
        public async Task<IActionResult> GetRequestJson(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return Unauthorized();

            var r = await _db.BloodRequests
                             .AsNoTracking()
                             .FirstOrDefaultAsync(x => x.Id == id);

            if (r == null) return NotFound();

            // return JSON shaped for client easily
            var outObj = new
            {
                id = r.Id,
                hospitalId = r.HospitalId,
                bloodGroup = r.BloodGroup,
                units = r.Units,
                personName = r.PersonName,
                mobileNumber = r.MobileNumber,
                address = r.Address,
                status = r.Status,
                requestedAt = r.RequestedAt.ToString("o") // ISO format
            };

            return Json(outObj);
        }

        // POST - update existing request
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRequest(BloodRequest input)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToAction("Login", "Account");

            var existing = await _db.BloodRequests.FindAsync(input.Id);
            if (existing == null) return NotFound();

            existing.HospitalId = input.HospitalId;
            existing.BloodGroup = input.BloodGroup;
            existing.Units = input.Units;
            existing.PersonName = input.PersonName;
            existing.MobileNumber = input.MobileNumber;
            existing.Address = input.Address;
            existing.Status = input.Status;

            // parse RequestedAt if sent (input.RequestedAt is DateTime)
            existing.RequestedAt = input.RequestedAt;

            await _db.SaveChangesAsync();

            TempData["Success"] = "Request updated successfully.";
            return RedirectToAction("Index", new { mode = "orders" });
        }

        // POST - delete request (safe)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("Username")))
                return RedirectToAction("Login", "Account");

            var req = await _db.BloodRequests.FindAsync(id);
            if (req == null) return NotFound();

            _db.BloodRequests.Remove(req);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Request deleted.";
            return RedirectToAction("Index", new { mode = "orders" });
        }


    }
}
