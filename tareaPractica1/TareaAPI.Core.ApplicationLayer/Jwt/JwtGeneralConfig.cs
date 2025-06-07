using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TareaAPI.Core.DomainLayer.Models;

namespace TareaAPI.InfrastructureLayer.Persistance.Utilities
{
    public class JwtGeneralConfig
    {
        
            private readonly IConfiguration _configuration;
            public JwtGeneralConfig(IConfiguration configuration)
            {
                _configuration = configuration;
            }

            public string encriptarSHA256(string texto)
            {

                using (SHA256 sha256Hash = SHA256.Create())
                {
                    //Computar Hash
                    byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(texto));


                    //Convertir array de bytes a string
                    StringBuilder builder = new StringBuilder();
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        builder.Append(bytes[i].ToString("X2"));
                    }

                    return builder.ToString();
                }

            }



            public string generateJWT(UserModel modelo)
            {
                //Crear la informacion del usuario para el token

                var userClaims = new[]
                {
                new Claim(ClaimTypes.NameIdentifier,modelo.Id.ToString()),
                new Claim(ClaimTypes.Name,modelo.Name!),
                new Claim(ClaimTypes.Email,modelo.Email!),
                 new Claim(ClaimTypes.DateOfBirth, modelo.BirthDate.ToString("yyyy-MM-dd") ?? "")
            };

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]!));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                //Crear detalle del token
                var jwtConfig = new JwtSecurityToken(
                    claims: userClaims,
                    expires: DateTime.UtcNow.AddMinutes(30),
                    signingCredentials: credentials
                    );

                return new JwtSecurityTokenHandler().WriteToken(jwtConfig);
            }


            public string GenerateRefreshToken()
            {
                var byteArray = new byte[64];

                var refreshToken = "";

                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(byteArray);

                    refreshToken = Convert.ToBase64String(byteArray);
                }

                return refreshToken;
            }

        }
    
}
