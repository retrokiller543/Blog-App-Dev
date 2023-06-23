using Microsoft.AspNetCore.Identity;

namespace Blog_App_Dev.Models
{
    public class HomePageViewModel
    {
        public List<BlogPost> LatestPosts { get; set; }
        public List<IdentityUser> TopUsers { get; set; }
    }

}
