using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt_ISS_be.Models
{
    [Table("projects")]
    public class Project
    {
        [Key]
        [Column("project_id")]
        public int ProjectId { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("description")]
        public string Description { get; set; }

        // Foreign Key to User
        [Required]
        [Column("user_id")]
        public int UserId { get; set; }

        // Navigation property
        [ForeignKey("UserId")]
        public User? User { get; set; }
    }
}

