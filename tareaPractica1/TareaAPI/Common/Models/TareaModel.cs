using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TareaAPI.Common.Models
{
    public class TareaModel<T>
    {
        public int IdTarea { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public T? AdditionalData { get; set; }

    }
}
