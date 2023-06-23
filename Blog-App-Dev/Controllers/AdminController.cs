using Blog_App_Dev.Data;
using Blog_App_Dev.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Blog_App_Dev.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ManageUsers()
        {
            var users = _userManager.Users.ToList();
            var model = new List<UserViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.Add(new UserViewModel { User = user, Roles = roles });
            }

            return View(model);
        }

        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();
            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Roles = roles,
                AllRoles = allRoles.Select(r => r.Name).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();
            var addedRoles = model.Roles.Except(userRoles);
            var removedRoles = userRoles.Except(model.Roles);

            await _userManager.AddToRolesAsync(user, addedRoles);
            await _userManager.RemoveFromRolesAsync(user, removedRoles);

            return RedirectToAction("ManageUsers");
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("ManageUsers");
            }
            else
            {
                // Handle the case when the deletion was not successful
                // For example, add the errors to the ModelState and return the same view
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(user);
            }
        }

        public async Task<IActionResult> ManagePosts()
        {
            var posts = await _context.BlogPosts.Include(p => p.User).ToListAsync();
            return View(posts);
        }

        public async Task<IActionResult> EditPost(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> EditPost(int id, BlogPost post)
        {
            if (id != post.ID)
            {
                return BadRequest();
            }

            _context.Entry(post).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManagePosts));
        }

        public async Task<IActionResult> DeletePost(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        [HttpPost, ActionName("DeletePost")]
        public async Task<IActionResult> DeletePostConfirmed(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            _context.BlogPosts.Remove(post);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManagePosts));
        }

        public IActionResult DeleteAllPosts()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllPostsConfirmed()
        {
            var comments = _context.CommentPosts.ToList();

            // Delete all comments
            _context.CommentPosts.RemoveRange(comments);
            await _context.SaveChangesAsync();

            // Get all blog posts
            var blogPosts = _context.BlogPosts.ToList();

            // Delete all blog posts
            _context.BlogPosts.RemoveRange(blogPosts);
            await _context.SaveChangesAsync();

            // Redirect the user to the main page.
            return RedirectToAction(nameof(Index));
        }

        public IActionResult ClearDatabase()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearDatabaseConfirmed()
        {
            // Get all users
            var users = _userManager.Users.ToList();
            // Get all blog posts
            var blogPosts = _context.BlogPosts.ToList();
            // Get all comments
            var comments = _context.CommentPosts.ToList();

            // Delete all comments
            _context.CommentPosts.RemoveRange(comments);
            await _context.SaveChangesAsync();

            // Delete all blog posts
            _context.BlogPosts.RemoveRange(blogPosts);
            await _context.SaveChangesAsync();

            // Delete all users
            foreach (var user in users)
            {
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    ModelState.AddModelError("", "Error while deleting user.");
                    return View();
                }
            }

            return RedirectToAction(nameof(Index));
        }


    }
}
