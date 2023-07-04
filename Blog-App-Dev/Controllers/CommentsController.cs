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
using Microsoft.AspNetCore.Authorization;

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

        // POST: Comments/Create
        [Authorize]
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

        // POST: Comments/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,Content,DatePosted,UserID,PostID")] Comment comment)
        {
            var currentUser = this.User;
            var currentUserID = _userManager.GetUserId(currentUser);
            if (id != comment.ID)
            {
                return NotFound();
            }

            if (currentUserID != comment.UserID && !(currentUser.IsInRole("Admin")))
            {
                return Unauthorized();
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
                return RedirectToAction("Details", "BlogPosts", new { id = comment.PostID });
            }
            ViewData["PostID"] = new SelectList(_context.BlogPosts, "ID", "Content", comment.PostID);
            ViewData["UserID"] = new SelectList(_context.Set<ApplicationUser>(), "Id", "Id", comment.UserID);
            return View(comment);
        }

        [Authorize]
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

            var currentUser = this.User;
            var currentUserID = _userManager.GetUserId(currentUser);
            if (currentUserID == comment.UserID || currentUser.IsInRole("Admin"))
            {
                return View(comment);
            }
            return Unauthorized();
        }

        [Authorize]
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
            int? postId = comment?.PostID;
            var currentUser = this.User;
            var currentUserID = _userManager.GetUserId(currentUser);
            if (comment != null && (currentUserID == comment.UserID || currentUser.IsInRole("Admin")))
            {
                _context.CommentPosts.Remove(comment);
            }
            
            await _context.SaveChangesAsync();
            if (postId == null)
            {
                return Problem();
            } else
            {
                return RedirectToAction("Details", "BlogPosts", new { id = postId });
            }

        }

        private bool CommentExists(int id)
        {
          return (_context.CommentPosts?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
