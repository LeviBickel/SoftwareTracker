using SoftwareTracker.Models;

namespace SoftwareTracker.Data
{
    public class LicenseHelper
    {
        readonly ApplicationDbContext _context;


        public LicenseHelper(ApplicationDbContext context)
        {
            _context = context;
        }

        //Check the license database once a day:
        //If the license expiration is going to expire before the next scan.. Send an email based on the last user who added 
        //the license. Email is their username.
        public List<LicenseModel> LicenseScan()
        {
            List<LicenseModel> license = new List<LicenseModel>();
            return license;
        }

    }
}
