using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SoftwareTracker.Data;
using SoftwareTracker.Models;
using Auth0.ManagementApi.Models;

namespace SoftwareTracker.Controllers
{
    [Authorize(Roles = "Administrators")]
    public class UserAdministrationController : Controller
    {
        private readonly Auth0UserService _auth0UserService;
        private readonly ILogger<UserAdministrationController> _logger;

        public UserAdministrationController(ILogger<UserAdministrationController> logger)
        {
            _auth0UserService = new Auth0UserService();
            _logger = logger;
        }

        // GET: UserAdministration
        public async Task<IActionResult> Index()
        {
            List<UserAdministration> usersToView = new List<UserAdministration>();
            var auth0Users = await _auth0UserService.GetAllUsersAsync();

            foreach (var user in auth0Users)
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

            var user = await _auth0UserService.GetUserByIdAsync(id);
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
                if (id == null)
                {
                    return NotFound();
                }

                var user = await _auth0UserService.GetUserByIdAsync(id);
                var translatedUser = await TranslateUserToView(user);
                if (user == null || translatedUser == null)
                {
                    return NotFound();
                }
                return View(translatedUser);
            }
            catch (Exception ex)
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
            var user = await _auth0UserService.GetUserByIdAsync(id);
            if (id == null || id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var changes = LoggingHelpers.EnumeratePropertyDifferences(await TranslateUserToView(user), userAdministration);
                try
                {
                    // Update user metadata in Auth0
                    var userUpdateRequest = new UserUpdateRequest
                    {
                        UserName = userAdministration.UserName,
                        Email = userAdministration.UserEmail,
                        Blocked = !userAdministration.CanLockout ? false : user.Blocked // Preserve blocked status unless modified
                    };

                    await _auth0UserService.UpdateUserAsync(id, userUpdateRequest);

                    // Handle role changes
                    var currentRoles = await _auth0UserService.GetUserRolesAsync(id);
                    var currentRoleName = currentRoles.FirstOrDefault()?.Name ?? "Users";

                    if (currentRoleName != userAdministration.Role)
                    {
                        // Note: You'll need to get role IDs from Auth0 dashboard or create a method to fetch them
                        // This is a simplified version - you may need to enhance this based on your Auth0 setup
                        _logger.LogWarning($"Role change requested from {currentRoleName} to {userAdministration.Role} for user {user.UserName}. Manual role assignment may be required in Auth0 dashboard.");
                    }

                    _logger.LogCritical($"{User.Identity.Name}, has modified the following user: {user.UserName}. Changes: {changes.Humanize()}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message);
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

            var user = await _auth0UserService.GetUserByIdAsync(id);
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
            var user = await _auth0UserService.GetUserByIdAsync(id);
            if (user != null)
            {
                await _auth0UserService.DeleteUserAsync(id);
                _logger.LogCritical($"{User.Identity.Name} has deleted the following user: {user.UserName}");
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<UserAdministration> TranslateUserToView(User user)
        {
            string userRole = "Users";
            var roles = await _auth0UserService.GetUserRolesAsync(user.UserId);

            if (roles.Any(r => r.Name == "Administrators"))
            {
                userRole = "Administrators";
                ViewBag.Role = new List<SelectListItem>() {
                    new SelectListItem { Text = "Administrators", Value = "Administrators" },
                    new SelectListItem { Text = "Users", Value = "Users" },
                };
            }
            else
            {
                ViewBag.Role = new List<SelectListItem>() {
                    new SelectListItem { Text = "Users", Value = "Users" },
                    new SelectListItem { Text = "Administrators", Value = "Administrators" },
                };
            }

            UserAdministration translatedUser = new UserAdministration
            {
                Id = user.UserId,
                UserName = user.UserName ?? user.Email,
                UserEmail = user.Email,
                CanLockout = true, // Auth0 uses "Blocked" status instead
                LockOutEndDate = user.Blocked == true ? DateTimeOffset.MaxValue : null,
                Role = userRole
            };
            return translatedUser;
        }

        public async Task<IActionResult> UnlockUserAccount(string id)
        {
            var user = await _auth0UserService.GetUserByIdAsync(id);
            if (id == null || id != user.UserId)
            {
                return NotFound();
            }
            try
            {
                await _auth0UserService.UnblockUserAsync(id);
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
            var user = await _auth0UserService.GetUserByIdAsync(id);
            if (id == null || id != user.UserId)
            {
                return NotFound();
            }
            try
            {
                await _auth0UserService.BlockUserAsync(id);
                _logger.LogCritical($"{User.Identity.Name} has locked {user.UserName}'s account");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
