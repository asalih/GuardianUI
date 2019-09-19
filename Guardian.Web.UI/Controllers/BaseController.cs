using Microsoft.AspNetCore.Mvc;

namespace Guardian.Web.UI.Controllers
{
    public class BaseController : Controller
    {
        public void Alert(string alertType, string message)
        {
            TempData["AlertType"] = alertType;
            TempData["Message"] = message;
        }
    }
}