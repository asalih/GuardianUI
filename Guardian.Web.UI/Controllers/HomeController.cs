using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Guardian.Web.UI.Models;
using Guardian.Domain;
using Microsoft.AspNetCore.Hosting;

namespace Guardian.Web.UI.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IWebHostEnvironment env;

        public HomeController(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public IActionResult Index()
        {
            return View();
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
