using Microsoft.AspNetCore.Mvc;

namespace Blog_App_Dev.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 400:
                    ViewBag.ErrorMessage = "Bad Request - The server could not understand the request due to invalid syntax.";
                    break;
                case 401:
                    ViewBag.ErrorMessage = "Unauthorized - The client must authenticate itself to get the requested response.";
                    break;
                case 403:
                    ViewBag.ErrorMessage = "Forbidden - The client does not have access rights to the content.";
                    break;
                case 404:
                    ViewBag.ErrorMessage = "Not Found - The server can not find the requested resource.";
                    break;
                case 500:
                    ViewBag.ErrorMessage = "Internal Server Error - The server has encountered a situation it doesn't know how to handle.";
                    break;
                case 502:
                    ViewBag.ErrorMessage = "Bad Gateway - The server was acting as a gateway or proxy and received an invalid response from the upstream server.";
                    break;
                case 503:
                    ViewBag.ErrorMessage = "Service Unavailable - The server is not ready to handle the request.";
                    break;
                default:
                    ViewBag.ErrorMessage = "An unexpected error occurred.";
                    break;
            }
            return View("StatusCode");
        }
    }

}
