using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Blog_App_Dev.Data;
using Blog_App_Dev.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Metadata;
using X.PagedList;

namespace Blog_App_Dev.Controllers
{
    public class BlogPostsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public BlogPostsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Blogs
        public async Task<IActionResult> Index()
        {
            var latestPosts = _context.BlogPosts
                                      .OrderByDescending(p => p.DatePosted)
                                      .Take(20)
                                      .ToList();

            var topUsers = _context.BlogPosts
                                   .GroupBy(p => p.UserID)
                                   .Select(group => new
                                   {
                                       UserId = group.Key,
                                       PostCount = group.Count()
                                   })
                                   .OrderByDescending(u => u.PostCount)
                                   .Take(5)
                                   .Select(u => u.UserId)
                                   .ToList();

            var topUsersWithDetails = _context.Users.OfType<ApplicationUser>() 
                                                  .Where(u => topUsers.Contains(u.Id))
                                                  .ToList();

            foreach (var user in topUsersWithDetails)
            {
                await _context.Entry(user)
                    .Collection(u => u.Posts).LoadAsync();
                await _context.Entry(user)
                    .Collection(u => u.Comments).LoadAsync();

                // Count the number of blog posts and comments.
                var postCount = user?.Posts?.Count;
                var commentCount = user?.Comments?.Count;

                // Pass these to the view.
                ViewBag.PostCount = postCount;
                ViewBag.CommentCount = commentCount;
            }

            var model = new HomePageViewModel
            {
                LatestPosts = latestPosts,
                TopUsers = topUsersWithDetails
            };

            return View(model);
        }

        public Task<IActionResult> Posts(int? page)
        {
            var pageNumber = page ?? 1;  // if no page was specified in the querystring, default to the first page (1)
            var pageSize = 12; // you can adjust the page size however you like
            var posts = _context.BlogPosts.ToPagedList(pageNumber, pageSize);

            return Task.FromResult<IActionResult>(View(posts));
        }


        // GET: Blogs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Blogs/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content")] BlogPost blog)
        {
            if (ModelState.IsValid)
            {
                var currentUser = this.User;
                blog.UserID = _userManager.GetUserId(currentUser);
                _context.Add(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(blog);
        }
        [Authorize]
        // GET: BlogPosts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .Include(b => b.User)
                .Include(b => b.Comments) // Include the comments related to the blog post
                .FirstOrDefaultAsync(m => m.ID == id);
            if (blogPost == null)
            {
                return NotFound();
            }

            ViewBag.PostID = blogPost.ID;

            return View(blogPost);

        }
        [Authorize]
        // GET: BlogPosts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts.FindAsync(id);
            if (blogPost == null)
            {
                return NotFound();
            }
            var currentUser = this.User;
            var currentUserID = _userManager.GetUserId(currentUser);
            if (blogPost.UserID == currentUserID)
            {
                return View(blogPost);
            }
            return RedirectToAction(nameof(Index));
            
        }

        // POST: BlogPosts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,Content,DatePosted")] BlogPost blogPost)
        {
            var currentUser = this.User;
            var currentUserID = _userManager.GetUserId(currentUser);
            if (id != blogPost.ID && currentUserID != blogPost.UserID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(blogPost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogPostExists(blogPost.ID))
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
            return View(blogPost);
        }
        [Authorize]
        // GET: BlogPosts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.BlogPosts == null)
            {
                return NotFound();
            }

            var blogPost = await _context.BlogPosts
                .FirstOrDefaultAsync(m => m.ID == id);
            if (blogPost == null)
            {
                return NotFound();
            }
            var currentUser = this.User;
            var currentUserID = _userManager.GetUserId(currentUser);
            if (currentUserID ==  blogPost.UserID)
            {
                return View(blogPost);
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: BlogPosts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.BlogPosts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.BlogPosts'  is null.");
            }
            var blogPost = await _context.BlogPosts.FindAsync(id);
            var currentUser = this.User;
            var currentUserID = _userManager.GetUserId(currentUser);
            if (blogPost != null && currentUserID == blogPost.UserID)
            {
                _context.BlogPosts.Remove(blogPost);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogPostExists(int id)
        {
          return (_context.BlogPosts?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
