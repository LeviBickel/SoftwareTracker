using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftwareTracker.Data;
using SoftwareTracker.Models;

namespace SoftwareTracker.Controllers
{
    [Authorize]
    public class LicenseController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LicenseController> _logger;

        public LicenseController(ApplicationDbContext context, ILogger<LicenseController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: License
        public async Task<IActionResult> Index()
        {
            return View(await _context.Licenses.ToListAsync());
        }

        // GET: License/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var licenseModel = await _context.Licenses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (licenseModel == null)
            {
                return NotFound();
            }

            return View(licenseModel);
        }

        // GET: License/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: License/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Manufacturer,SoftwareTitle,AssignedServer,PurchaseOrder,PurchaseDate,LicenseType,Support,SupportExp,AmountofKeys,UsedKeys,RemainingKeys,LicenseKey")] LicenseModel licenseModel)
        {
            if (ModelState.IsValid)
            {
                var changes = LoggingHelpers.EnumeratePropertyDifferences(new LicenseModel() 
                { 
                    Manufacturer = "new",
                    SoftwareTitle = "new",
                    AssignedServer = "new",
                    PurchaseOrder = "new",
                    LicenseKey = "new",
                    LicenseType = "new",
                    PurchaseDate = new DateTime(0),
                    Support = false,
                    SupportExp = new DateTime(0),
                    AmountofKeys = 0,
                    UsedKeys = 0,
                    RemainingKeys = 0,
                }, licenseModel).Humanize();
                _context.Add(licenseModel);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"{User.Identity.Name} added a new license: {changes.Humanize()}");
                return RedirectToAction(nameof(Index));
            }
            return View(licenseModel);
        }

        // GET: License/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var licenseModel = await _context.Licenses.FindAsync(id);
            if (licenseModel == null)
            {
                return NotFound();
            }
            return View(licenseModel);
        }

        // POST: License/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Manufacturer,SoftwareTitle,AssignedServer,PurchaseOrder,PurchaseDate,LicenseType,Support,SupportExp,AmountofKeys,UsedKeys,RemainingKeys,LicenseKey")] LicenseModel licenseModel)
        {
            if (id != licenseModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var changes = LoggingHelpers.EnumeratePropertyDifferences(_context.Licenses.AsNoTracking().FirstOrDefault(m=>m.Id == licenseModel.Id), licenseModel);
                try
                {
                    _context.Update(licenseModel);
                    await _context.SaveChangesAsync();
                    _logger.LogCritical($"{User.Identity.Name} modified license with ID: {licenseModel.Id} with the following changes: {changes.Humanize()}");
                }
                catch (Exception ex)
                {
                    if (!LicenseModelExists(licenseModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        _logger.LogError(ex.Message);
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(licenseModel);
        }

        // GET: License/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var licenseModel = await _context.Licenses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (licenseModel == null)
            {
                return NotFound();
            }

            return View(licenseModel);
        }

        // POST: License/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var licenseModel = await _context.Licenses.FindAsync(id);
            if (licenseModel != null)
            {
                _logger.LogCritical($"{User.Identity.Name} is deleting License with Id: {licenseModel.Id}");
                _context.Licenses.Remove(licenseModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LicenseModelExists(int id)
        {
            return _context.Licenses.Any(e => e.Id == id);
        }
    }
}
