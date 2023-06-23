using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blog_App_Dev.Data;
using Blog_App_Dev.Models;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Metadata;

namespace Blog_App_Dev.Controllers
{
    public class CommentsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly ApplicationDbContext _context;

        public CommentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Comments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.CommentPosts.Include(c => c.Post).Include(c => c.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.CommentPosts == null)
            {
                return NotFound();
            }

            var comment = await _context.CommentPosts
                .Include(c => c.Post)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comments/Create
        public IActionResult Create(int postId)
        {
            ViewBag.PostID = postId;
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int postID, [Bind("ID,Title,Content")] Comment comment)
        {
            comment.PostID = postID;
            if (ModelState.IsValid)
            {
                var currentUser = this.User;
                comment.UserID = _userManager.GetUserId(currentUser);
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "BlogPosts", new { id = postID });
            }
            return View(comment);
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.CommentPosts == null)
            {
                return NotFound();
            }

            var comment = await _context.CommentPosts.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["PostID"] = new SelectList(_context.BlogPosts, "ID", "Content", comment.PostID);
            ViewData["UserID"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", comment.UserID);
            return View(comment);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,Content,DatePosted,UserID,PostID")] Comment comment)
        {
            if (id != comment.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PostID"] = new SelectList(_context.BlogPosts, "ID", "Content", comment.PostID);
            ViewData["UserID"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", comment.UserID);
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.CommentPosts == null)
            {
                return NotFound();
            }

            var comment = await _context.CommentPosts
                .Include(c => c.Post)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.CommentPosts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.CommentPosts'  is null.");
            }
            var comment = await _context.CommentPosts.FindAsync(id);
            if (comment != null)
            {
                _context.CommentPosts.Remove(comment);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
          return (_context.CommentPosts?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
