using Microsoft.AspNetCore.Mvc;

namespace WebApplication16.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
