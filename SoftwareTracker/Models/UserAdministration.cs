namespace SoftwareTracker.Models
{
    public class UserAdministration
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public DateTimeOffset? LockOutEndDate { get; set; }
        public bool CanLockout { get; set; }
        public string Role { get; set; }
    }
}
