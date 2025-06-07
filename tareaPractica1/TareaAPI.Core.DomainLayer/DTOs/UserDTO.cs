using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TareaAPI.Core.DomainLayer.DTOs
{
    public class UserDTO
    {
        [MinLength(3)]
        [MaxLength(30)]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
