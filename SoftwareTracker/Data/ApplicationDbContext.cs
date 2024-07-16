using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SoftwareTracker.Models;

namespace SoftwareTracker.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
           
        }
        public DbSet<LicenseModel> Licenses { get; set; }
        public DbSet<UserAdministration> userAdministration {  get; set; }

        public DbSet<ArchivalModel> Archival { get; set; }
    }
}
