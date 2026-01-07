using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Projekt_ISS_be.Models
{
    [Table("status")]
    public class Status
    {
        [Key]
        [Column("status_id")]
        public int StatusId { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("name")]
        public string Name { get; set; } = null!;
    }
}

