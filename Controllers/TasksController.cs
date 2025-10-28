using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt_ISS_be.Data;
using Projekt_ISS_be.Models;
using System.Security.Claims;
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

        public IActionResult Create() => View();

        // 🟡 CREATE (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Models.Task newTask)
        {
            var userId = User.FindFirstValue("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Authentication");

            try
            {
                _context.Tasks.Add(newTask);
                await _context.SaveChangesAsync();

                ViewBag.Message = "Task created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewBag.Error = "Error creating task.";
                return View(newTask);
            }
        }

        // 🔵 EDIT (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var userId = User.FindFirstValue("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Authentication");

            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.TaskId == id);

            if (task == null)
            {
                ViewBag.Error = "Task not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(task);
        }

        // 🔵 EDIT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Models.Task updatedTask)
        {
            var userId = User.FindFirstValue("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Authentication");

            var existingTask = await _context.Tasks
                .FirstOrDefaultAsync(t => t.TaskId == updatedTask.TaskId);

            if (existingTask == null)
            {
                ViewBag.Error = "Task not found.";
                return View(updatedTask);
            }

            existingTask.Title = updatedTask.Title;
            existingTask.Description = updatedTask.Description;

            try
            {
                _context.Update(existingTask);
                await _context.SaveChangesAsync();

                ViewBag.Message = "Task updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewBag.Error = "Error updating task.";
                return View(updatedTask);
            }
        }

        // 🔴 DELETE (GET)
        public async Task<IActionResult> Delete(int id)
        {
            var userId = User.FindFirstValue("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Authentication");

            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.TaskId == id);

            if (task == null)
            {
                ViewBag.Error = "Task not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(task);
        }

        // 🔴 DELETE (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Authentication");

            var task = await _context.Tasks
                .FirstOrDefaultAsync(t => t.TaskId == id);

            if (task == null)
            {
                ViewBag.Error = "Task not found.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();

                ViewBag.Message = "Task deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ViewBag.Error = "Error deleting task.";
                return View(task);
            }
        }

    }
}
