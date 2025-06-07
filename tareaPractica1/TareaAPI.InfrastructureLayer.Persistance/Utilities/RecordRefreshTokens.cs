using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TareaAPI.Core.DomainLayer.Models;

namespace TareaAPI.InfrastructureLayer.Persistance.Utilities
{
    public class RecordRefreshTokens
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdRecordToken { get; set; }

        // Clave Foranea que referencia a User
        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual UserModel User { get; set; }

        [Required]
        public string Token { get; set; }


        [Required]
        public string RefreshToken { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}
