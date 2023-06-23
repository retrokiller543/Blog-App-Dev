namespace Blog_App_Dev.Models
{
    public class UserViewModel
    {
        public ApplicationUser User { get; set; }
        public IList<string> Roles { get; set; }
    }

}
