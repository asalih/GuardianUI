using Microsoft.AspNetCore.Mvc;

namespace Guardian.Web.UI.Controllers
{
    public class BaseController : Controller
    {
        public void Alert(string alertType, string message)
        {
            ViewBag.AlertType = alertType;
            ViewBag.Message = message;
        }
    }
}