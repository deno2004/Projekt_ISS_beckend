using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt_ISS_be.Models
{
    [Table("tasks")]

    public class Task
    {
        [Key]
        [Column("task_id")]
        public int TaskId { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("description")]
        public string Description { get; set; }

    }
}
