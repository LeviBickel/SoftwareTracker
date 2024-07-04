using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<IdentityUser> _userManager;

        public LicenseController(ApplicationDbContext context, ILogger<LicenseController> logger, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: License
        public async Task<IActionResult> Index()
        {
            List<LicenseModel> licenses = await _context.Licenses.Where(m=>m.AddedBy == _userManager.GetUserId(User)).ToListAsync();
            foreach(var license in licenses)
            {
                license.LicenseKey= EncryptionHelper.Decrypt(license.LicenseKey);
            }
            return View(licenses);
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
            licenseModel.LicenseKey = EncryptionHelper.Decrypt(licenseModel.LicenseKey);
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
        public async Task<IActionResult> Create([Bind("Id,Manufacturer,SoftwareTitle,AssignedServer,PurchaseOrder,PurchaseDate,LicenseType,LicenseExp,Support,SupportExp,AmountofKeys,UsedKeys,RemainingKeys,LicenseKey,NotifyOnLicExp,NotifyOnSupExp")] LicenseModel licenseModel)
        {
            licenseModel.AddedBy = _userManager.GetUserId(User);
            if (licenseModel.AddedBy != null) { ModelState["AddedBy"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid; }
            
            if (ModelState.IsValid)
            {
                licenseModel.LicenseKey = EncryptionHelper.Encrypt(licenseModel.LicenseKey);
                licenseModel.Notified = false;
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
                    AddedBy = "new",
                    LicenseExp = new DateTime(0),
                    Notified = false,
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
            licenseModel.LicenseKey = EncryptionHelper.Decrypt(licenseModel.LicenseKey);
            return View(licenseModel);
        }

        // POST: License/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Manufacturer,SoftwareTitle,AssignedServer,PurchaseOrder,PurchaseDate,LicenseType,LicenseExp,Support,SupportExp,AmountofKeys,UsedKeys,RemainingKeys,LicenseKey,AddedBy,NotifyOnLicExp,NotifyOnSupExp")] LicenseModel licenseModel)
        {
            if (id != licenseModel.Id)
            {
                return NotFound();
            }
            licenseModel.AddedBy = _userManager.GetUserId(User);
            if (licenseModel.AddedBy != null) { ModelState["AddedBy"].ValidationState = Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Valid; }
            if (ModelState.IsValid)
            {
                licenseModel.LicenseKey = EncryptionHelper.Encrypt(licenseModel.LicenseKey);
                licenseModel.Notified = _context.Licenses.AsNoTracking().FirstOrDefault(m=>m.Id == licenseModel.Id).Notified;
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
            licenseModel.LicenseKey = EncryptionHelper.Decrypt(licenseModel.LicenseKey);
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
