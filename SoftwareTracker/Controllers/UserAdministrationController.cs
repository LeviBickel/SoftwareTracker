using Humanizer;
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
        private readonly ILogger<UserAdministrationController> _logger;

        public UserAdministrationController(ApplicationDbContext context, UserManager<IdentityUser> userManager, ILogger<UserAdministrationController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
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

        // GET: UserAdministration/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                ViewBag.Role = new List<SelectListItem>() {
                new SelectListItem { Text = "Administrators", Value = "Administrators" },
                new SelectListItem { Text = "Users", Value = "Users" },
            };

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
            } catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: UserAdministration/Edit/5
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
                var changes = LoggingHelpers.EnumeratePropertyDifferences(await TranslateUserToView(user), userAdministration);
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
                   
                    _logger.LogCritical($"{User.Identity.Name}, has modified the following user: {user.UserName}. Changes: {changes.Humanize()}");
                }
                catch (Exception ex)
                {
                    if (!UserAdministrationExists(userAdministration.Id))
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
            _logger.LogCritical($"{User.Identity.Name} has deleted the following user: {user.UserName}");
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

        public async Task<IActionResult> UnlockUserAccount(string id)
        {
            IdentityUser user = await _context.Users.FindAsync(id);
            if (id == null || id != user.Id)
            {
                return NotFound();
            }
            try
            {   
                user.LockoutEnd = null;
                user.AccessFailedCount = 0;
                _context.Update(user);
                await _context.SaveChangesAsync();
                _logger.LogCritical($"{User.Identity.Name} has unlocked {user.UserName}'s account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> LockUserAccount(string id)
        {
            IdentityUser user = await _context.Users.FindAsync(id);
            if (id == null || id != user.Id)
            {
                return NotFound();
            }
            try
            {
                user.LockoutEnd = DateTime.Now.Date.AddYears(500);
                user.AccessFailedCount = 5;
                _context.Update(user);
                await _context.SaveChangesAsync();
                _logger.LogCritical($"{User.Identity.Name} has locked {user.UserName}'s account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool UserAdministrationExists(string id)
        {
            return _context.userAdministration.Any(e => e.Id == id);
        }
    }
}
