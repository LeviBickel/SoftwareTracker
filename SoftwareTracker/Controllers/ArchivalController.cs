using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftwareTracker.Data;
using SoftwareTracker.Models;

namespace SoftwareTracker.Controllers
{
    public class ArchivalController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ArchivalController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Archival
        public async Task<IActionResult> Index()
        {
            var archivedLicenses = _context.Archival.Where(m => m.AddedBy == _userManager.GetUserId(User)).Take(500).ToList();
            foreach(var license in archivedLicenses)
            {
                license.LicenseKey = EncryptionHelper.Decrypt(license.LicenseKey);
            }
            return View(archivedLicenses);
        }

        // GET: Archival/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var archivalModel = await _context.Archival
                .FirstOrDefaultAsync(m => m.Id == id);
            if (archivalModel == null)
            {
                return NotFound();
            }

            return View(archivalModel);
        }

        // GET: Archival/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Archival/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Manufacturer,SoftwareTitle,AssignedServer,PurchaseOrder,PurchaseDate,LicenseType,LicenseExp,Support,SupportExp,AmountofKeys,UsedKeys,RemainingKeys,LicenseKey,AddedBy,Notified,DeletedOn")] ArchivalModel archivalModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(archivalModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(archivalModel);
        }

        // GET: Archival/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var archivalModel = await _context.Archival.FindAsync(id);
            if (archivalModel == null)
            {
                return NotFound();
            }
            return View(archivalModel);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Manufacturer,SoftwareTitle,AssignedServer,PurchaseOrder,PurchaseDate,LicenseType,LicenseExp,Support,SupportExp,AmountofKeys,UsedKeys,RemainingKeys,LicenseKey,AddedBy,Notified,DeletedOn")] ArchivalModel archivalModel)
        {
            if (id != archivalModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(archivalModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArchivalModelExists(archivalModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(archivalModel);
        }

        // GET: Archival/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var archivalModel = await _context.Archival
                .FirstOrDefaultAsync(m => m.Id == id);
            if (archivalModel == null)
            {
                return NotFound();
            }

            return View(archivalModel);
        }

        // POST: Archival/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var archivalModel = await _context.Archival.FindAsync(id);
            if (archivalModel != null)
            {
                _context.Archival.Remove(archivalModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArchivalModelExists(int id)
        {
            return _context.Archival.Any(e => e.Id == id);
        }
    }
}
