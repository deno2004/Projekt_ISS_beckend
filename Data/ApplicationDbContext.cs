using Projekt_ISS_be.Models;
using Microsoft.EntityFrameworkCore;

namespace Projekt_ISS_be.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> Users { get; set; } 
        public DbSet<Models.Task> Tasks { get; set; }
    }
}
