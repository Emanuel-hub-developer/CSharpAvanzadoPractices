using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TareaAPI.Controllers;
using TareaAPI.Core.DomainLayer.DTOs;
using TareaAPI.Core.DomainLayer.Models;
using TareaAPI.Infrastructure.Data;
using TareaAPI.InfrastructureLayer.Persistance.Utilities;

namespace TestUnit
{
    public class JwtGeneralConfigTests
    {
        private readonly IConfiguration _configuration;

        public JwtGeneralConfigTests()
        {

            var inMemorySettings = new Dictionary<string, string> {
            {"Jwt:key", "claveSuperSecretaParaTests1234567890!"}
        };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        //Test 6 
        [Fact]
        public void GenerateJWT_DeberiaGenerarTokenValido()
        {
            // Arrange
            var jwtConfig = new JwtGeneralConfig(_configuration);

            var user = new UserModel
            {
                Id = 42,
                Name = "UsuarioTest",
                Email = "test@correo.com",
                BirthDate = new DateTime(1990, 5, 15)
            };

            // Act
            string token = jwtConfig.generateJWT(user);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(token));

            var handler = new JwtSecurityTokenHandler();
            Assert.True(handler.CanReadToken(token));

            var jwtToken = handler.ReadJwtToken(token);

            // Verificar claims basicos
            Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.NameIdentifier && c.Value == user.Id.ToString());
            Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Name && c.Value == user.Name);
            Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.Email && c.Value == user.Email);
            Assert.Contains(jwtToken.Claims, c => c.Type == ClaimTypes.DateOfBirth && c.Value == user.BirthDate.ToString("yyyy-MM-dd"));

            // Verificar expiracion (aprox 30 minutos desde ahora)
            Assert.True(jwtToken.ValidTo > DateTime.UtcNow);
            Assert.True(jwtToken.ValidTo <= DateTime.UtcNow.AddMinutes(31));
        }


        //Test 7 
        [Fact]
        public void GenerateRefreshToken_DeberiaGenerarTokenNoNuloYDe64Bytes()
        {
            // Arrange
            var clase = new JwtGeneralConfig(_configuration);

            // Act
            string token = clase.GenerateRefreshToken();

            // Assert
            Assert.False(string.IsNullOrEmpty(token)); // Token no debe ser null ni vacío


            // Convertir base64 a bytes para verificar longitud
            byte[] bytes = Convert.FromBase64String(token);
            Assert.Equal(64, bytes.Length); // Debe ser exactamente 64 bytes
        }


        
    }
}
