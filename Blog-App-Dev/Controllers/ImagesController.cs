using Blog_App_Dev.Data;
using Microsoft.AspNetCore.Mvc;
using Unsplash;
using Unsplash.Api;

namespace Blog_App_Dev.Controllers
{
    public class ImagesController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly IConfiguration _configuration;


        public ImagesController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            string accessKey = _configuration["UnsplashAccessKey"];

            var client = new UnsplashClient(new ClientOptions
            {
                AccessKey = accessKey,
            });

            int page = 1;

            var photos = await client.Photos.GetPhotosAsync(new Unsplash.Api.FilterOptions((int)page, perPage: 20, orderBy: Unsplash.Api.PhotoOrderBy.Popular));
            ViewBag.Page = page;

            return View(photos);
        }

        [HttpPost]
        public async Task<IActionResult> Index(int page)
        {
            string accessKey = _configuration["UnsplashAccessKey"];

            var client = new UnsplashClient(new ClientOptions
            {
                AccessKey = accessKey,
            });

            var photos = await client.Photos.GetPhotosAsync(new Unsplash.Api.FilterOptions((int)page, perPage: 20, orderBy: Unsplash.Api.PhotoOrderBy.Popular));

            ViewBag.Page = page;

            return View(photos);
        }

        public async Task<IActionResult> SearchImage(string query)
        {
            string accessKey = _configuration["UnsplashAccessKey"];

            var clientOptions = new ClientOptions
            {
                AccessKey = accessKey,
            };

            var searchApi = new SearchApi(clientOptions);

            var searchParams = new SearchPhotosParams
            {
                
            };

            var photos = await searchApi.PhotosAsync(query, searchParams);

            ViewBag.Query = query;

            return View(photos.Results);
        }

    }
}
