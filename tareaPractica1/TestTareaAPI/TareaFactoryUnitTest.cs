using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TareaAPI.Factory;

namespace TestUnit
{
    public class TareaFactoryUnitTest
    {
        //Test 8 
        [Fact]
        public void CrearTareaBasica_DebeCrearTareaConPropiedadesCorrectas()
        {
            // Arrange
            string description = "Test tarea básica";
            DateTime dueDate = new DateTime(2025, 12, 31);
            string status = "Pendiente";
            string additionalData = "Dato adicional";

            // Act
            var tarea = TareaFactory.CrearTareaBasica(description, dueDate, status, additionalData);

            // Assert
            Assert.NotNull(tarea);
            Assert.Equal(description, tarea.Description);
            Assert.Equal(dueDate, tarea.DueDate);
            Assert.Equal(status, tarea.Status);
            Assert.Equal(additionalData, tarea.AdditionalData);
        }


        //Test 9 
        [Fact]
        public void CrearTareaUrgente_DebeCrearTareaConPropiedadesCorrectas()
        {
            // Arrange
            string description = "Test tarea urgente";
            DateTime expectedDueDate = DateTime.Now.AddDays(1);

            // Act
            var tarea = TareaFactory.CrearTareaUrgente(description);

            // Assert
            Assert.NotNull(tarea);
            Assert.Equal(description, tarea.Description);
            Assert.Equal("Pendiente", tarea.Status);
            Assert.Equal("Prioridad alta", tarea.AdditionalData);

            // Para fecha, verificamos que la diferencia porque DateTime.Now puede variar
            Assert.True((tarea.DueDate - expectedDueDate).TotalSeconds < 1);
        }
    }
}
