using Castle.Core.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using TareaAPI.Controllers;
using TareaAPI.Core.DomainLayer.DTOs;
using TareaAPI.Core.DomainLayer.Models;
using TareaAPI.Hubs;
using TareaAPI.Infrastructure.Data;
using TareaAPI.Infrastructure.Entities;
using TareaAPI.InfrastructureLayer.Persistance.Utilities;
using TareaAPI.Utilities;

namespace TestTareaAPI
{
    public class UnitTestFirstEndPoints
    {
        //Test 1 
        [Fact]
        public async Task GetTareas_DeberiaRetornarListaDeTareas()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TareaAPIContext>()
                .UseInMemoryDatabase(databaseName: "GetTareasDB")
                .Options;

            using var context = new TareaAPIContext(options);
            context.Tareas.Add(new TareaEntity { IdTarea = 1, Description = "Test", DueDate = DateTime.Now.AddDays(1), Status = "Pendiente" });
            await context.SaveChangesAsync();

            var mockTaskQueue = new Mock<TaskQueueManager>();
            var mockHubContext = new Mock<IHubContext<NotificationHub>>();

            var controller = new TareaController(context, mockTaskQueue.Object, mockHubContext.Object);

            // Act
            var result = await controller.GetTareas();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var tareas = Assert.IsAssignableFrom<IEnumerable<TareaEntity>>(okResult.Value);
            Assert.Single(tareas);
        }


        //Test 2 FUNCIONA
        [Fact]
        public async Task GetTareasPorStatus_DeberiaRetornarTareasFiltradasPorStatus()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TareaAPIContext>()
                .UseInMemoryDatabase(databaseName: "GetTareasPorStatusDB")
                .Options;

            using var context = new TareaAPIContext(options);
            context.Tareas.AddRange(
                new TareaEntity { IdTarea = 1, Description = "Test1", DueDate = DateTime.Now.AddDays(1), Status = "Pendiente" },
                new TareaEntity { IdTarea = 2, Description = "Test2", DueDate = DateTime.Now.AddDays(2), Status = "Completado" }
            );
            await context.SaveChangesAsync();

            var mockTaskQueue = new Mock<TaskQueueManager>();
            var mockHubContext = new Mock<IHubContext<NotificationHub>>();

            var controller = new TareaController(context, mockTaskQueue.Object, mockHubContext.Object);

            // Act
            var result = await controller.GetTareasPorStatus("Pendiente");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var tareas = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.Single(tareas);
        }

        

        //Test 3
        [Fact]
        public async Task UpdateTarea_TareaNoExiste_DeberiaRetornarNotFound()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TareaAPIContext>()
                .UseInMemoryDatabase(databaseName: "UpdateTareaNoExisteDB")
                .Options;

            using var context = new TareaAPIContext(options);
            var mockTaskQueue = new Mock<TaskQueueManager>();
            var mockHubContext = new Mock<IHubContext<NotificationHub>>();

            var controller = new TareaController(context, mockTaskQueue.Object, mockHubContext.Object);

            var tareaActualizada = new TareaEntity
            {
                Description = "Actualizado",
                DueDate = DateTime.Now.AddDays(5),
                Status = "Pendiente",
                AdditionalData = "Datos"
            };

            // Act
            var result = await controller.UpdateTarea(999, tareaActualizada);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            var json = System.Text.Json.JsonSerializer.Serialize(notFoundResult.Value);
            var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, System.Text.Json.JsonElement>>(json);

            bool success = dict["success"].GetBoolean();
            string message = dict["message"].GetString();

            Assert.False(success);
            Assert.Equal("Tarea no encontrada.", message);
        }


        //Test 4 
        [Fact]
        public async Task DeleteTarea_DeberiaEliminarTareaCorrectamente()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TareaAPIContext>()
                .UseInMemoryDatabase(databaseName: "DeleteTareaDB")
                .Options;

            using var context = new TareaAPIContext(options);
            context.Tareas.Add(new TareaEntity
            {
                IdTarea = 1,
                Description = "Tarea a eliminar",
                DueDate = DateTime.Now.AddDays(1),
                Status = "Pendiente"
            });
            await context.SaveChangesAsync();

            var mockTaskQueue = new Mock<TaskQueueManager>();
            var mockHubContext = new Mock<IHubContext<NotificationHub>>();

            var controller = new TareaController(context, mockTaskQueue.Object, mockHubContext.Object);

            // Act
            var result = await controller.DeleteTarea(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            // Deserializamos en diccionario de JsonElement para luego extraer los valores correctamente
            var json = System.Text.Json.JsonSerializer.Serialize(okResult.Value);
            var dict = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, System.Text.Json.JsonElement>>(json);

            bool success = dict["success"].GetBoolean();
            string message = dict["message"].GetString();

            Assert.True(success);
            Assert.Equal("Tarea eliminada correctamente", message);
        }

        //Test 5 
        [Fact]
        public async Task ObtenerPorcentajeCompletado_DeberiaRetornarPorcentajeCorrecto()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<TareaAPIContext>()
                .UseInMemoryDatabase(databaseName: "DB_PorcentajeCompletado")
                .Options;

            using var context = new TareaAPIContext(options);

            context.Tareas.AddRange(
                new TareaEntity { Description = "Tarea 1", DueDate = DateTime.Now, Status = "Completada" },
                new TareaEntity { Description = "Tarea 2", DueDate = DateTime.Now, Status = "Completada" },
                new TareaEntity { Description = "Tarea 3", DueDate = DateTime.Now, Status = "Pendiente" }
            );
            await context.SaveChangesAsync();

            var mockQueue = new TaskQueueManager(); // Puedes usar instancia real o mock si quieres
            var mockHub = new Mock<IHubContext<NotificationHub>>();

            var controller = new TareaController(context, mockQueue, mockHub.Object);

            // Act
            var result = await controller.ObtenerPorcentajeCompletado();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            var json = JsonSerializer.Serialize(okResult.Value);
            var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

            Assert.True(data.ContainsKey("porcentaje"));
            Assert.True(data.ContainsKey("mensaje"));

            double porcentaje = data["porcentaje"].GetDouble();

            Assert.Equal(66.67, Math.Round(porcentaje, 2));
        }




    }

}


    

