using Microsoft.AspNetCore.Mvc;

namespace Wasalnyy.PL.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
