using Microsoft.AspNetCore.Identity;
using SoftwareTracker.Models;
using System.Runtime.CompilerServices;

namespace SoftwareTracker.Data
{
    public class LicenseHelper
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly EmailSender _emailSender;

        public LicenseHelper(ApplicationDbContext context, ILogger logger, UserManager<IdentityUser> userManager, EmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        //Check the license database once a day:
        //If the license expiration is going to expire before the next scan.. Send an email based on the last user who added 
        //the license. Email is their username.
        public async void LicenseScan()
        {
            List<LicenseModel> licenses = _context.Licenses.ToList();
            try
            {
                if(licenses.Count <= 0)
                {
                    return; //Fix this -> 
                }
                //there are licenses here. We can do something with them.
                foreach(var license in licenses) 
                {
                    if (license.LicenseExp <= DateTime.Now.AddDays(1))
                    {
                        //the license is set to expire within the next day... send an email to the responsible party.
                        //this class should provide: email, subject, message
                        var user = await _userManager.FindByIdAsync(license.AddedBy);
                        string emailAddress = user.UserName;
                        string subject = "Your License is going to expire";
                        string message = $"This is an automated notification that your {license.Manufacturer} license is going to expire within 24 hours.";
                        await _emailSender.SendEmailAsync(emailAddress,subject,message);
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
            }
                
            return;
        }

    }
}
