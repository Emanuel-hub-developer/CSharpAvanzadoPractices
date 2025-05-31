using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using TareaAPI.Factory;
using TareaAPI.Infrastructure.Data;
using TareaAPI.Infrastructure.Entities;
using TareaAPI.InfrastructureLayer.Persistance.Utilities;
using TareaAPI.Utilities;

namespace TareaAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    public class TareaController : Controller
    {
        private readonly TareaAPIContext _context;
        private readonly TaskQueueManager _taskQueueManager;

        delegate bool ValidarTarea(TareaEntity task);


        public TareaController(TareaAPIContext context,TaskQueueManager taskManagerQueue)
        {
            _context = context;
            _taskQueueManager = taskManagerQueue;
        }

        [HttpGet]
        [Route("obtenerTareas")]
        public async Task<IActionResult> GetTareas()
        {
            try
            {

                var tareas = await _context.Tareas.ToListAsync();
               
                if(tareas == null)
                {
                    return NotFound(new { success = false, message = "No hay tareas registradas." });
                }
               
                return Ok(tareas);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener las tareas {ex.Message}");
                return StatusCode(500, "Ocurrio un error al obtener las tareas");
            }

        }

        [HttpGet]
        [Route("obtenerPorStatus")]
        public async Task<IActionResult> GetTareasPorStatus(string statusName)
        {
            if (string.IsNullOrWhiteSpace(statusName))
            {
                return BadRequest("El nombre del status es requerido.");
            }

            var tareas = await _context.Tareas
                .Where(t => t.Status == statusName)
                .Select(t => new
                {
                    t.IdTarea,
                    t.Description,
                    t.DueDate,
                    t.Status,
                    t.AdditionalData
                })
                .ToListAsync();

            if (tareas == null || !tareas.Any())
            {
                return NotFound($"No se encontraron tareas con el status '{statusName}'.");
            }

            return Ok(tareas);
        }

        [HttpGet]
        [Route("obtenerPorcentajeCompletado")]
        public async Task<IActionResult> ObtenerPorcentajeCompletado()
        {
            try
            {
                var tareas = await _context.Tareas.ToListAsync();
                if (tareas == null || !tareas.Any())
                {
                    return NotFound("No hay tareas disponibles para calcular el porcentaje.");
                }

                double porcentaje = TaskAnalitics.CalcularPorcentajeCompletado(tareas);
                return Ok(new
                {
                    porcentaje,
                    mensaje = $"El Porcentaje de tareas completadas es: {porcentaje:F2}%"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al calcular el porcentaje: {ex.Message}");
                return StatusCode(500, "Ocurrio un error al calcular el porcentaje de tareas completadas.");
            }
        }

        [HttpPost]
        [Route("crearTarea")]
        public async Task<IActionResult> CreateTarea(TareaEntity tareaEntity)
        {
            var tarea = TareaFactory.CrearTareaBasica(
            tareaEntity.Description,
            tareaEntity.DueDate,
            tareaEntity.Status,
            tareaEntity.AdditionalData
        );

            if (tarea == null)
            {
                return NotFound();
            }

            try
            {
                ValidarTarea validar = task => !string.IsNullOrWhiteSpace(task.Description) && task.DueDate > DateTime.Now;

                Func<TareaEntity,int> calculateDaysLeft = t => (t.DueDate - DateTime.Now).Days;

                Func<TareaEntity,string> notifyCreation = task =>
                {
                    int daysLeft = calculateDaysLeft(task);

                    return $"Tarea creada: {task.Description}, vencimiento: {task.DueDate}, dias restantes: {daysLeft}";
                };
              

                if (!validar(tarea))
                {
                    return BadRequest("Tarea invalida");
                }

                var tcs = new TaskCompletionSource<bool>();

                _taskQueueManager.Enqueue(async () =>
                {
                    await _context.Tareas.AddAsync(tarea);
                    await _context.SaveChangesAsync();
                    tcs.SetResult(true);
                });

                await tcs.Task;

                var mensaje = $"Tarea creada exitosamente.{notifyCreation(tarea)}";

                return Ok(new
                {
                    success = true,

                    message = mensaje
                });

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear la tarea {ex.Message}");

                return StatusCode(500, "Ocurrio un error al crear la tarea");
            }
        }

        [HttpPut]
        [Route("actualizarTarea")]

        public async Task<IActionResult> UpdateTarea(int id, TareaEntity tareaEntity)
        {

            var tarea = await _context.Tareas.FirstOrDefaultAsync(t => t.IdTarea == id);


            if (tarea == null)
            {
                return NotFound(new { success = false, message = "Tarea no encontrada." });
            }

            tarea.Description = tareaEntity.Description;
            tarea.DueDate = tareaEntity.DueDate;
            tarea.Status = tareaEntity.Status;
            tarea.AdditionalData = tareaEntity.AdditionalData;


            try
            {
         
                ValidarTarea validar = task => !string.IsNullOrWhiteSpace(task.Description) && task.DueDate > DateTime.Now;

                if (!validar(tarea))
                {
                    return BadRequest("Tarea invalida");
                }

                Func<TareaEntity, int> calculateDaysLeft = t => (t.DueDate - DateTime.Now).Days;

                int diasRestantes = calculateDaysLeft(tarea);

                Func<TareaEntity,string> notifyUpdate = task =>
                {
                   return$"Tarea actualizada: {task.Description}, nueva fecha de vencimiento: {task.DueDate}, dias restantes: {diasRestantes}";
                };

                await _context.SaveChangesAsync();


                var mensaje = $"Tarea creada exitosamente.\n{notifyUpdate(tarea)}";

                return Ok(new
                {
                    success = true,

                    message = mensaje
                });

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error al actualizar la tarea: {ex.Message}");
                return StatusCode(500, "Ocurrio un error al actualizar la tarea.");
            }
        }



        [HttpDelete]
        [Route("eliminarTarea")]
        public async Task<IActionResult> DeleteTarea(int id)
        {

            var tarea = await _context.Tareas.FirstOrDefaultAsync(t => t.IdTarea == id);

            if (tarea == null)
            {

                return NotFound(new { success = false, message = "Tarea no encontrada." });
            }

            try
            {
                var tareaEliminada = new
                {
                    tarea.IdTarea,
                    tarea.Description,
                    tarea.DueDate,
                    tarea.Status,
                    tarea.AdditionalData
                };

                _context.Tareas.Remove(tarea);


                await _context.SaveChangesAsync();


                return Ok(new { success = true, message = "Tarea eliminada correctamente", data = tareaEliminada });

            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error al eliminar la tarea: {ex.Message}");
                return StatusCode(500, "Ocurrió un error al eliminar la tarea.");
            }

        }
    }
}
