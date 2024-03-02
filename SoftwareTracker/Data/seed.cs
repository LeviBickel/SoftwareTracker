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

        public static void SeedUsers(UserManager<IdentityUser> userManager, ApplicationDbContext context, ILogger logger)
        {
            try
            {
                if (!context.Users.Any(u => u.Email == "admin@invoicer.invoicer"))
                {
                    var userStore = new UserStore<IdentityUser>(context);
                    //var manager = new UserManager<IdentityUser>(userStore);
                    var user = new IdentityUser() { UserName = "admin@invoicer.invoicer", Email = "admin@invoicer.invoicer", EmailConfirmed = true, LockoutEnabled= false };
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
        }
    }
}
