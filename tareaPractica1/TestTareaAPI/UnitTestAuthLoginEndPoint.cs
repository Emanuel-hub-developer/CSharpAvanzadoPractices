using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class UnitTestAuthLoginEndPoint
    {
        //Test 10 

        [Fact]
        public async Task Login_CredencialesValidas_DeberiaRetornarTokens()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TareaAPIContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> { { "Jwt:key", "clave_secreta_para_testing1234567890" } })
                .Build();

            using var context = new TareaAPIContext(options);
            var jwtConfig = new JwtGeneralConfig(configuration);

            var passwordPlano = "123456";
            var passwordHasheada = jwtConfig.encriptarSHA256(passwordPlano);

            var usuario = new UserModel
            {
                Name = "Test",
                Email = "test@example.com",
                BirthDate = DateTime.Now.AddYears(-20),
                Password = passwordHasheada
            };

            context.Users.Add(usuario);
            await context.SaveChangesAsync();

            var controller = new AccessController(context, jwtConfig);

            var loginDTO = new LoginDTO
            {
                Email = "test@example.com",
                Password = passwordPlano
            };

            // Act
            var resultado = await controller.Login(loginDTO);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(resultado);
            var data = okResult.Value;

            var json = JsonSerializer.Serialize(data);
            var parsed = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

            Assert.True(parsed.ContainsKey("Token"));
            Assert.True(parsed.ContainsKey("RefreshToken"));
            Assert.False(string.IsNullOrEmpty(parsed["Token"].GetString()));
            Assert.False(string.IsNullOrEmpty(parsed["RefreshToken"].GetString()));
        }
    }
}
