using Microsoft.AspNetCore.Identity;

namespace FFive.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsEnabled { get; set; }
    }
}