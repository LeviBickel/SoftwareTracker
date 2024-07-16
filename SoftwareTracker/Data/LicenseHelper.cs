using Hangfire;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using SoftwareTracker.Models;

namespace SoftwareTracker.Data
{
    public class LicenseHelper
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LicenseHelper> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly EmailSender _emailSender;

        public LicenseHelper(ApplicationDbContext context, ILogger<LicenseHelper> logger, UserManager<IdentityUser> userManager, EmailSender emailSender)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        //Check the license database once a day:
        //If the license expiration is going to expire before the next scan.. Send an email based on the last user who added 
        //the license. Email is their username.
        public async Task LicenseScan()
        {
            List<LicenseModel> licenses = _context.Licenses.Where(m=>m.LicenseExp <= DateTime.Now.AddDays(1) && m.Notified == false).ToList();
            try
            {
                if(licenses.Count <= 0)
                {
                    return; // there are no licenses returned from the database. 
                }
                

                //there are licenses here. We can do something with them.
                foreach(var license in licenses.Where(m=>m.NotifyOnLicExp == true || m.NotifyOnSupExp == true)) 
                {
                    //Check for license expirations:
                    if (license.LicenseExp <= DateTime.Now.AddDays(1) && license.LicenseExp >= DateTime.Today && license.NotifyOnLicExp == true)
                    {
                        //the license is set to expire within the next day... send an email to the responsible party.
                        //this class should provide: email, subject, message
                        var user = await _userManager.FindByIdAsync(license.AddedBy);
                        string emailAddress = user.UserName;
                        string subject = $"Your {license.Manufacturer} license is going to expire";
                        string message = $"This is an automated notification that your {license.Manufacturer} - {license.SoftwareTitle} license is going to expire on {license.LicenseExp}";
                        await _emailSender.SendEmailAsync(emailAddress,subject,message);
                    }
                    else if(license.LicenseExp <= DateTime.Today && license.NotifyOnLicExp == true)
                    {
                        //The license has already expired. Send an expiration message and change the notified flag so we don't send them an email every day.
                        var user = await _userManager.FindByIdAsync(license.AddedBy);
                        string emailAddress = user.UserName;
                        string subject = $"Your {license.Manufacturer} license has expired";
                        string message = $"This is an automated notification that your {license.Manufacturer} - {license.SoftwareTitle} license expired on {license.LicenseExp}";
                        await _emailSender.SendEmailAsync(emailAddress, subject, message);
                        license.Notified = true;
                        _context.Update(license);
                        await _context.SaveChangesAsync();
                    }

                    //Check for support expirations:
                    if(license.Support == true && license.SupportExp <= DateTime.Now.AddDays(1) && license.NotifyOnSupExp == true){
                        var user = await _userManager.FindByIdAsync(license.AddedBy);
                        string emailAddress = user.UserName;
                        string subject = $"Support for {license.Manufacturer} license is going to expire";
                        string message = $"This is an automated notification that support for your {license.Manufacturer} - {license.SoftwareTitle} license is going to expire on {license.SupportExp}";
                        await _emailSender.SendEmailAsync(emailAddress,subject,message);
                    }
                    else if (license.SupportExp <= DateTime.Today && license.NotifyOnSupExp == true){
                        var user = await _userManager.FindByIdAsync(license.AddedBy);
                        string emailAddress = user.UserName;
                        string subject = $"Support for your {license.Manufacturer} license has expired";
                        string message = $"This is an automated notification that support for your {license.Manufacturer} - {license.SoftwareTitle} license expired on {license.SupportExp}";
                        await _emailSender.SendEmailAsync(emailAddress, subject, message);
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
