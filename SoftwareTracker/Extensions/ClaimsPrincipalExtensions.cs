using System.Security.Claims;

namespace SoftwareTracker.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Gets the Auth0 user ID from the ClaimsPrincipal.
        /// This replaces UserManager.GetUserId() for Auth0 authentication.
        /// </summary>
        public static string GetAuth0UserId(this ClaimsPrincipal principal)
        {
            // Auth0 stores the user ID in the NameIdentifier claim
            return principal?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }
}
