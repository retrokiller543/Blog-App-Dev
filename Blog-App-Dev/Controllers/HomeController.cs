using Blog_App_Dev.Data;
using Blog_App_Dev.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Unsplash.Models;

namespace Blog_App_Dev.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;


        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var latestPosts = _context.BlogPosts
                                      .OrderByDescending(p => p.DatePosted)
                                      .Take(4)
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

            var topUsersWithDetails = _context.Users.OfType<ApplicationUser>() // This line is changed
                                                  .Where(u => topUsers.Contains(u.Id))
                                                  .ToList();

            foreach (var user in topUsersWithDetails) // This line is changed
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



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}