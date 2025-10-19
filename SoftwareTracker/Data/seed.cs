using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace SoftwareTracker.Data
{
    public class Seeder
    {
        public static async Task CreateRoles(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager, ILogger logger)
        {
            try
            {
                string[] roleNames = { "Administrators", "Users" };
                IdentityResult roleResult;

                foreach (var roleName in roleNames)
                {
                    var roleExists = await roleManager.RoleExistsAsync(roleName);
                    if (!roleExists)
                    {
                        roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                    }
                }
            }
            catch (Exception ex)
            {
                //var logger = serviceProvider.GetService<ILogger>();
                logger.LogError(ex.ToString());
                return;
            }
        }

        // NOTE: This seeder is for Microsoft Identity and is no longer used with Auth0
        // User management is now handled through Auth0 Dashboard
        public static void SeedUsers(UserManager<IdentityUser> userManager, ApplicationDbContext context, ILogger logger)
        {
            // This method is deprecated - Auth0 users are managed in Auth0 Dashboard
            logger.LogWarning("SeedUsers is deprecated. Please manage users through Auth0 Dashboard.");

            /* Commented out - no longer compatible with Auth0
            try
            {
                if (!context.Users.Any(u => u.Email == "admin@software.tracker"))
                {
                    var userStore = new UserStore<IdentityUser>(context);
                    //var manager = new UserManager<IdentityUser>(userStore);
                    var user = new IdentityUser() { UserName = "admin@software.tracker", Email = "admin@software.tracker", EmailConfirmed = true, LockoutEnabled= false };
                    IdentityResult result = userManager.CreateAsync(user, "Password123!!").Result;
                    if (result.Succeeded)
                    {
                        userManager.AddToRoleAsync(user, "Administrators").Wait();
                    }
                }

            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return;
            }
            */
        }
    }
}
