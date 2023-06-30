using Blog_App_Dev.Data;
using Blog_App_Dev.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog_App_Dev.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();

            return View(users);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Load the related BlogPosts and Comments.
            await _context.Entry(user)
                .Collection(u => u.Posts).LoadAsync();
            await _context.Entry(user)
                .Collection(u => u.Comments).LoadAsync();

            // Count the number of blog posts and comments.
            var postCount = user.Posts.Count;
            var commentCount = user.Comments.Count;

            // Pass these to the view.
            ViewBag.PostCount = postCount;
            ViewBag.CommentCount = commentCount;

            return View(user);
        }
    }
}
