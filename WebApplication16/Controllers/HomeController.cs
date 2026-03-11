using Microsoft.AspNetCore.Mvc;

namespace WebApplication16.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();

        public IActionResult Privacy() => View();
    }
}