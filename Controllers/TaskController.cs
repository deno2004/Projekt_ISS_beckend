using Microsoft.AspNetCore.Mvc;

namespace Projekt_ISS_be.Controllers
{
    public class TaskController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
