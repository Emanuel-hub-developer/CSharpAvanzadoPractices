using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TareaAPI.Infrastructure.Entities
{
    public class TareaEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdTarea { get; set; }

        [MinLength(7)]
        [MaxLength(150)]
        public string Description { get; set; }

        public DateTime DueDate { get; set; }

        [MaxLength(30)]
        public string Status { get; set; }

        [MaxLength(100)]
        public string? AdditionalData { get; set; }
    }
}
