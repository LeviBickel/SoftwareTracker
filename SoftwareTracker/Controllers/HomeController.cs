using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftwareTracker.Data;
using SoftwareTracker.Extensions;
using SoftwareTracker.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace SoftwareTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.User = User.FindFirstValue(ClaimTypes.Email);

            // Add stats for authenticated users
            if (User.Identity.IsAuthenticated)
            {
                var userId = User.GetAuth0UserId();
                var licenses = await _context.Licenses
                    .AsNoTracking()
                    .Where(m => m.AddedBy == userId)
                    .ToListAsync();

                ViewBag.TotalLicenses = licenses.Count;
                ViewBag.ExpiringLicenses = licenses.Count(l => l.LicenseExp <= DateTime.Now.AddDays(30) && l.LicenseExp >= DateTime.Now);
                ViewBag.ActiveLicenses = licenses.Count(l => l.LicenseExp > DateTime.Now);
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
