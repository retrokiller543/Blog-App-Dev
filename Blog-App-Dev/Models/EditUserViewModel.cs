namespace Blog_App_Dev.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
        public IList<string> AllRoles { get; set; }
    }

}
