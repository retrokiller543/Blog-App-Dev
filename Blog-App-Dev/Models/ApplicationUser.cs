using Microsoft.AspNetCore.Identity;

namespace Blog_App_Dev.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<BlogPost>? Posts { get; set; }
    }
}
