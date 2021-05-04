using Microsoft.AspNetCore.Mvc;

namespace GeoFinder.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
