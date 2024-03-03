using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoftwareTracker.Data;
using SoftwareTracker.Models;

namespace SoftwareTracker.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class UserAdministrationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UserAdministrationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UserAdministration
        public async Task<IActionResult> Index()
        {
            return View(await _context.userAdministration.ToListAsync());
        }

        // GET: UserAdministration/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAdministration = await _context.userAdministration
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userAdministration == null)
            {
                return NotFound();
            }

            return View(userAdministration);
        }

        // GET: UserAdministration/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserAdministration/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserName,UserEmail,LockOutEndDate,CanLockout,Role")] UserAdministration userAdministration)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userAdministration);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userAdministration);
        }

        // GET: UserAdministration/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAdministration = await _context.userAdministration.FindAsync(id);
            if (userAdministration == null)
            {
                return NotFound();
            }
            return View(userAdministration);
        }

        // POST: UserAdministration/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,UserEmail,LockOutEndDate,CanLockout,Role")] UserAdministration userAdministration)
        {
            if (id != userAdministration.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userAdministration);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserAdministrationExists(userAdministration.Id))
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
            return View(userAdministration);
        }

        // GET: UserAdministration/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userAdministration = await _context.userAdministration
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userAdministration == null)
            {
                return NotFound();
            }

            return View(userAdministration);
        }

        // POST: UserAdministration/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var userAdministration = await _context.userAdministration.FindAsync(id);
            if (userAdministration != null)
            {
                _context.userAdministration.Remove(userAdministration);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserAdministrationExists(string id)
        {
            return _context.userAdministration.Any(e => e.Id == id);
        }
    }
}
