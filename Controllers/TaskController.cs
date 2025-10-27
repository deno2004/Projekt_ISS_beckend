using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt_ISS_be.Data;
using System.Threading.Tasks;
using Projekt_ISS_be.Models;

namespace Projekt_ISS_be.Controllers
{
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TaskController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var tasks = _context.Tasks.ToList();
            return View(tasks);
        }


    }
}
