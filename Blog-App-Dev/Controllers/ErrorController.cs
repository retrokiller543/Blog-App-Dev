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
                    ViewBag.DefualtErrorMessage = "400: Bad Request - The server could not understand the request due to invalid syntax.";
                    ViewBag.StatusCode = "400";
                    break;
                case 401:
                    ViewBag.DefualtErrorMessage = "401: Unauthorized - The client must authenticate itself to get the requested response.";
                    ViewBag.StatusCode = "401";
                    break;
                case 403:
                    ViewBag.DefualtErrorMessage= "403: Forbidden - The client does not have access rights to the content.";
                    ViewBag.StatusCode = "403";
                    break;
                case 404:
                    ViewBag.DefualtErrorMessage= "404: Not Found - The server can not find the requested resource.";
                    ViewBag.StatusCode = "404";
                    break;
                case 500:
                    ViewBag.DefualtErrorMessage= "500: Internal Server Error - The server has encountered a situation it doesn't know how to handle.";
                    ViewBag.StatusCode = "500";
                    break;
                case 502:
                    ViewBag.DefualtErrorMessage= "502: Bad Gateway - The server was acting as a gateway or proxy and received an invalid response from the upstream server.";
                    ViewBag.StatusCode = "502";
                    break;
                case 503:
                    ViewBag.DefualtErrorMessage= "503: Service Unavailable - The server is not ready to handle the request.";
                    ViewBag.StatusCode = "503";
                    break;
                default:
                    ViewBag.DefualtErrorMessage= "An unexpected error occurred.";
                    break;
            }
            return View("StatusCode");
        }
    }

}
