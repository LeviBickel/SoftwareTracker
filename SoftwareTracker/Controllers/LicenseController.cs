using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoftwareTracker.Data;
using SoftwareTracker.Models;

namespace SoftwareTracker.Controllers
{
    public class LicenseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LicenseController(ApplicationDbContext context)
        {
            _context = context;
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Manufacturer,SoftwareTitle,AssignedServer,PurchaseOrder,PurchaseDate,LicenseType,Support,SupportExp,AmountofKeys,UsedKeys,RemainingKeys,LicenseKey")] LicenseModel licenseModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(licenseModel);
                await _context.SaveChangesAsync();
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
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
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
                try
                {
                    _context.Update(licenseModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LicenseModelExists(licenseModel.Id))
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
