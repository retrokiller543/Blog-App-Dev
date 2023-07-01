using Microsoft.AspNetCore.Mvc;

namespace Blog_App_Dev.Controllers
{
    public class HomeController1 : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
