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
using Markdig;
using Ganss.Xss;

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
      // First, fetch the data and order by DatePosted
      var latestPosts = _context.BlogPosts
          .OrderByDescending(p => p.DatePosted)
          .Take(12)
          .Include(u => u.User)
          .ToList();  // ToList is called here to fetch the data first

      // Then, apply the GetExcerpt method to each blog post
      latestPosts = latestPosts.Select(p => new BlogPost
      {
        // Copy over all other properties as needed
        ID = p.ID,
        Title = p.Title,
        Content = p.Content,
        DatePosted = p.DatePosted,
        User = p.User,
        // ...

        // Then calculate the Excerpt
        Excerpt = GetExcerpt(p.Content, 100)
      }).ToList();



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

      var posts = _context.BlogPosts
          .Include(p => p.User)
          .OrderByDescending(p => p.DatePosted)
          .Select(p => new BlogPost
          {
            ID = p.ID,
            Title = p.Title,
            Content = p.Content,
            Excerpt = GetExcerpt(p.Content, 100),
            // Assign other properties as well...
            User = p.User,
            DatePosted = p.DatePosted
          })
          .ToPagedList(pageNumber, pageSize);

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
    // GET: BlogPosts/Details/5
    [Authorize]
    public async Task<IActionResult> Details(int? id)
    {
      if (id == null || _context.BlogPosts == null)
      {
        return NotFound();
      }

      var blogPost = await _context.BlogPosts
          .Include(b => b.User)
          .Include(b => b.Comments)
              .ThenInclude(c => c.User)
          .FirstOrDefaultAsync(m => m.ID == id);
      if (blogPost == null)
      {
        return NotFound();
      }

      ViewBag.PostID = blogPost.ID;
      ViewData["Comment"] = new Comment();

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
      if (blogPost.UserID == currentUserID || currentUser.IsInRole("Admin"))
      {
        return View(blogPost);
      }

      ViewData["ErrorMessage"] = "You are not authorized to edit this blog post.";
      return Unauthorized();
    }

    // POST: BlogPosts/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("ID,Title,Content,DatePosted,UserID")] BlogPost blogPost)
    {
      var currentUser = this.User;
      var currentUserID = _userManager.GetUserId(currentUser);

      if (id != blogPost.ID)
      {
        return NotFound();
      }

      if (currentUserID != blogPost.UserID && !(currentUser.IsInRole("Admin")))
      {
        return Unauthorized();
      }

      if (ModelState.IsValid)
      {
        try
        {
          var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
          blogPost.Content = Markdown.ToHtml(blogPost.Content ?? string.Empty, pipeline);

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
      if (currentUserID == blogPost.UserID || currentUser.IsInRole("Admin"))
      {
        return View(blogPost);
      }
      return Unauthorized();
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
      if (blogPost != null && (currentUserID == blogPost.UserID || currentUser.IsInRole("Admin")))
      {
        var comments = _context.CommentPosts.Where(m => m.PostID == blogPost.ID).ToList();
        foreach (var comment in comments)
        {
          _context.CommentPosts.Remove(comment);
          await _context.SaveChangesAsync();
        }
        _context.BlogPosts.Remove(blogPost);
      }

      await _context.SaveChangesAsync();
      return RedirectToAction(nameof(Index));
    }

    private bool BlogPostExists(int id)
    {
      return (_context.BlogPosts?.Any(e => e.ID == id)).GetValueOrDefault();
    }
    private static string GetExcerpt(string content, int length)
    {
      if (string.IsNullOrEmpty(content))
      {
        return string.Empty;
      }

      // Trim to desired length
      var trimmedContent = content.Length <= length ? content : content.Substring(0, length);

      // Convert markdown to HTML
      var markdownPipeline = new Markdig.MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
      var htmlContent = Markdig.Markdown.ToHtml(trimmedContent, markdownPipeline);

      return htmlContent;
    }


  }
}
