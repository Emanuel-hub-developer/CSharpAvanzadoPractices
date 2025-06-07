using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TareaAPI.Core.DomainLayer.DTOs
{
    public class RefreshTokenDTO
    {
        public string? RefreshToken { get; set; }

        public string? Token { get; set; }
    }
}
