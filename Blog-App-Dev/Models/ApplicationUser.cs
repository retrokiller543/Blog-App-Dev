using Microsoft.AspNetCore.Identity;

namespace Blog_App_Dev.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime JoinedAt { get; set; } = DateTime.Now;
        public ICollection<BlogPost>? Posts { get; set; }

        public ICollection<Comment>? Comments { get; set; }
    }
}
