using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<IdentityUser> _userManager;

        public UserAdministrationController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: UserAdministration
        public async Task<IActionResult> Index()
        {
            List<UserAdministration> usersToView = new List<UserAdministration>();
            foreach (var user in _context.Users)
            {
                usersToView.Add(await TranslateUserToView(user));
            }
            return View(usersToView);
        }

        // GET: UserAdministration/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            var translatedUser = await TranslateUserToView(user);
            if (translatedUser == null || user == null)
            {
                return NotFound();
            }

            return View(translatedUser);
        }

        // GET: UserAdministration/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UserAdministration/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,UserName,UserEmail,LockOutEndDate,CanLockout,Role")] UserAdministration userAdministration)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(userAdministration);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(userAdministration);
        //}

        // GET: UserAdministration/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            var translatedUser = await TranslateUserToView(user);
            if (user == null || translatedUser == null)
            {
                return NotFound();
            }
            return View(translatedUser);
        }

        // POST: UserAdministration/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,UserEmail,LockOutEndDate,CanLockout,Role")] UserAdministration userAdministration)
        {
            IdentityUser user = await _context.Users.FindAsync(id);
            if (id == null || id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    user.UserName = userAdministration.UserName;
                    user.NormalizedEmail = userAdministration.UserEmail;
                    user.LockoutEnd = userAdministration.LockOutEndDate;
                    user.LockoutEnabled = userAdministration.CanLockout;

                    //Modify the users role if needed:
                    if (!_userManager.GetRolesAsync(user).Result.Contains(userAdministration.Role))
                    {
                        await _userManager.RemoveFromRoleAsync(user, _userManager.GetRolesAsync(user).Result.First());
                        await _userManager.AddToRoleAsync(user, userAdministration.Role);
                    }
                    _context.Update(user);
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

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            var translatedUser = await TranslateUserToView(user);
            if (user == null || translatedUser == null)
            {
                return NotFound();
            }

            return View(translatedUser);
        }

        // POST: UserAdministration/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private async Task<UserAdministration> TranslateUserToView(IdentityUser user)
        {
            string userRole = "Users";
            if (await _userManager.IsInRoleAsync(user, "Administrators"))
            {
                userRole = "Administrators";
            }
            UserAdministration translatedUser = new UserAdministration
            {
                Id = user.Id,
                UserName = user.UserName,
                UserEmail = user.NormalizedEmail,
                CanLockout = user.LockoutEnabled,
                LockOutEndDate = user.LockoutEnd,
                Role = userRole
            };
            return translatedUser;
        }

        private bool UserAdministrationExists(string id)
        {
            return _context.userAdministration.Any(e => e.Id == id);
        }
    }
}
