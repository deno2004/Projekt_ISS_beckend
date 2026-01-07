using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Projekt_ISS_be.Data;
using Projekt_ISS_be.Models;
using System.Security.Claims;

namespace Projekt_ISS_be.Controllers
{
    [Route("api/projects")]
    [ApiController]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/projects
        [HttpGet]
        public async Task<IActionResult> GetProjects()
        {
            try
            {
                Console.WriteLine("🔍 GetProjects called");

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine($"🔑 User ID from token: {userIdClaim}");

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    Console.WriteLine("❌ No user ID in token");
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var userId = int.Parse(userIdClaim);
                Console.WriteLine($"👤 Looking for projects with UserId: {userId}");

                var projects = await _context.Projects
                    .Where(p => p.UserId == userId)
                    .Select(p => new
                    {
                        id = p.ProjectId,
                        title = p.Title,
                        description = p.Description
                    })
                    .ToListAsync();

                Console.WriteLine($"✅ Found {projects.Count} projects");
                foreach (var p in projects)
                {
                    Console.WriteLine($"  - Project {p.id}: {p.title}");
                }

                return Ok(projects);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in GetProjects: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Error loading projects" });
            }
        }

        // POST: api/projects
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto dto)
        {
            try
            {
                Console.WriteLine("🔍 CreateProject called");
                Console.WriteLine($"📝 Title: {dto?.Title}");
                Console.WriteLine($"📝 Description: {dto?.Description}");

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine($"🔑 User ID from token: {userIdClaim}");

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    Console.WriteLine("❌ No user ID in token");
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var userId = int.Parse(userIdClaim);
                Console.WriteLine($"👤 Creating project for UserId: {userId}");

                if (string.IsNullOrWhiteSpace(dto?.Title))
                {
                    Console.WriteLine("❌ Title is required");
                    return BadRequest(new { message = "Title is required" });
                }

                var project = new Project
                {
                    Title = dto.Title,
                    Description = dto.Description ?? "",
                    UserId = userId
                };

                Console.WriteLine($"💾 Saving project to database...");
                _context.Projects.Add(project);
                await _context.SaveChangesAsync();

                Console.WriteLine($"✅ Project created with ID: {project.ProjectId}");

                return Ok(new
                {
                    id = project.ProjectId,
                    title = project.Title,
                    description = project.Description
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error in CreateProject: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                return StatusCode(500, new { message = "", error = ex.Message });
            }
        }



        // PUT: api/projects/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(int id, [FromBody] UpdateProjectDto dto)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var userId = int.Parse(userIdClaim);

                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.ProjectId == id && p.UserId == userId);

                if (project == null)
                {
                    return NotFound(new { message = "Project not found" });
                }

                project.Title = dto.Title;
                project.Description = dto.Description;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    id = project.ProjectId,
                    title = project.Title,
                    description = project.Description
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateProject: {ex.Message}");
                return StatusCode(500, new { message = "Error updating project" });
            }
        }

        // DELETE: api/projects/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(int id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized(new { message = "User not authenticated" });
                }

                var userId = int.Parse(userIdClaim);

                var project = await _context.Projects
                    .FirstOrDefaultAsync(p => p.ProjectId == id && p.UserId == userId);

                if (project == null)
                {
                    return NotFound(new { message = "Project not found" });
                }

                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Project deleted successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteProject: {ex.Message}");
                return StatusCode(500, new { message = "Error deleting project" });
            }
        }
    }

    public class CreateProjectDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }

    public class UpdateProjectDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}

