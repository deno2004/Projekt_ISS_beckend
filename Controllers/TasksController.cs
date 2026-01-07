using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt_ISS_be.Data;
using Projekt_ISS_be.Models;
using System.Security.Claims;

namespace Projekt_ISS_be.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TasksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/tasks
        [HttpGet]
        public async Task<IActionResult> GetTasks()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var userId = int.Parse(userIdClaim);

                var tasks = await _context.Tasks
                    .Where(t => t.UserId == userId)
                    .Select(t => new
                    {
                        id = t.TaskId,
                        title = t.Title,
                        description = t.Description
                    })
                    .ToListAsync();

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetTasks: {ex.Message}");
                return StatusCode(500, new { message = "Error loading tasks" });
            }
        }

        // POST: api/tasks
        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var userId = int.Parse(userIdClaim);

                var task = new Models.Task
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    UserId = userId,
                };

                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    id = task.TaskId,
                    title = task.Title,
                    description = task.Description
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateTask: {ex.Message}");
                return StatusCode(500, new { message = "Error creating task" });
            }
        }

        // PUT: api/tasks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var userId = int.Parse(userIdClaim);

                var task = await _context.Tasks
                    .FirstOrDefaultAsync(t => t.TaskId == id && t.UserId == userId);

                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                task.Title = dto.Title;
                task.Description = dto.Description;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    id = task.TaskId,
                    title = task.Title,
                    description = task.Description
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateTask: {ex.Message}");
                return StatusCode(500, new { message = "Error updating task" });
            }
        }

        // DELETE: api/tasks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var userId = int.Parse(userIdClaim);

                var task = await _context.Tasks
                    .FirstOrDefaultAsync(t => t.TaskId == id && t.UserId == userId);

                if (task == null)
                {
                    return NotFound(new { message = "Task not found" });
                }

                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Task deleted successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteTask: {ex.Message}");
                return StatusCode(500, new { message = "Error deleting task" });
            }
        }
    }

    public class CreateTaskDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class UpdateTaskDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}