using Blog_App_Dev.Data;
using Blog_App_Dev.Models;
using Microsoft.AspNetCore.Authorization;
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

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: Admin/EditPost/5
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts.FindAsync(id);

            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: Admin/EditPost/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id, [Bind("ID,Title,Content,DatePosted,UserID")] BlogPost blogPost)
        {
            if (id != blogPost.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(blogPost);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(blogPost);
        }

        // GET: Admin/DeletePost/5
        public async Task<IActionResult> DeletePost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .FirstOrDefaultAsync(m => m.ID == id);

            if (blogPost == null)
            {
                return NotFound();
            }

            return View(blogPost);
        }

        // POST: Admin/DeletePost/5
        [HttpPost, ActionName("DeletePost")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePostConfirmed(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);
            _context.BlogPosts.Remove(blogPost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ActionName("ClearAllData")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearAllDataConfirmed()
        {
            foreach (var entity in _context.Model.GetEntityTypes())
            {
                var records = _context.Set(entity.Name);
                _context.RemoveRange(records);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}
