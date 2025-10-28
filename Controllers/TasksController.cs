using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt_ISS_be.Data;
using Projekt_ISS_be.Models;
using System.Threading.Tasks;

namespace Projekt_ISS_be.Controllers
{
    public class TasksController : Controller
    {
        private readonly ApplicationDbContext _context;
        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var tasks = _context.Tasks.ToList();
            return View(tasks);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var tasks = await _context.Tasks.FirstOrDefaultAsync(t => t.TaskId == id);
            if (tasks == null)
                return NotFound();

            return View(tasks);
        }

        public async Task<IActionResult> Create()
        {
            var tasks = _context.Tasks.ToList();
            return View(tasks);
        }

        public async Task<IActionResult> Update()
        {
            var tasks = _context.Tasks.ToList();
            return View(tasks);
        }

        public async Task<IActionResult> Delete()
        {
            var tasks = _context.Tasks.ToList();
            return View(tasks);
        }

    }
}
