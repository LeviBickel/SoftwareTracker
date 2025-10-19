using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoftwareTracker.Data;
using SoftwareTracker.Data.Migrations;
using SoftwareTracker.Models;
using SoftwareTracker.Extensions;

namespace SoftwareTracker.Controllers
{
    public class ArchivalController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ArchivalController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Archival
        public async Task<IActionResult> Index()
        {
            var userId = User.GetAuth0UserId();

            // PERFORMANCE FIX: Use async and AsNoTracking
            var archivedLicenses = await _context.Archival
                .AsNoTracking()
                .Where(m => m.AddedBy == userId)
                .OrderByDescending(m => m.DeletedOn)
                .Take(500)
                .ToListAsync();

            // Decrypt in parallel for better performance
            Parallel.ForEach(archivedLicenses, license =>
            {
                license.LicenseKey = EncryptionHelper.Decrypt(license.LicenseKey);
            });

            return View(archivedLicenses);
        }

        // GET: Archival/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.GetAuth0UserId();

            // Single query with user validation
            var archivalModel = await _context.Archival
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.AddedBy == userId);

            if (archivalModel == null)
            {
                return NotFound();
            }

            archivalModel.LicenseKey = EncryptionHelper.Decrypt(archivalModel.LicenseKey);
            return View(archivalModel);
        }

        // GET: Archival/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.GetAuth0UserId();

            // Single query with user validation
            var archivalModel = await _context.Archival
                .FirstOrDefaultAsync(m => m.Id == id && m.AddedBy == userId);

            if (archivalModel == null)
            {
                return NotFound();
            }

            archivalModel.LicenseKey = EncryptionHelper.Decrypt(archivalModel.LicenseKey);
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
                    archivalModel.LicenseKey = EncryptionHelper.Encrypt(archivalModel.LicenseKey);
                    _context.Update(archivalModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ArchivalModelExistsAsync(archivalModel.Id))
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

            var userId = User.GetAuth0UserId();

            // Single query with user validation
            var archivalModel = await _context.Archival
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id && m.AddedBy == userId);

            if (archivalModel == null)
            {
                return NotFound();
            }

            archivalModel.LicenseKey = EncryptionHelper.Decrypt(archivalModel.LicenseKey);
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

        private async Task<bool> ArchivalModelExistsAsync(int id)
        {
            return await _context.Archival.AnyAsync(e => e.Id == id);
        }
    }
}
