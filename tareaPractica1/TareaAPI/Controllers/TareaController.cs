using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using TareaAPI.Common.Models;
using TareaAPI.Infrastructure.Data;
using TareaAPI.Infrastructure.Entities;

namespace TareaAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    public class TareaController : Controller
    {
        private readonly TareaAPIContext _context;

        public TareaController(TareaAPIContext context)
        {
            _context = context;

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

        [HttpPost]
        [Route("crearTarea")]
        public async Task<IActionResult> CreateTarea(TareaEntity tareaEntity)
        {
            var tarea = new TareaEntity
            {
                Description = tareaEntity.Description,
                DueDate = tareaEntity.DueDate,
                Status = tareaEntity.Status,
                AdditionalData = tareaEntity.AdditionalData
            };

            try
            {
                if (tarea == null)
                {
                    return NotFound();
                }

                await _context.Tareas.AddAsync(tarea);

                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(CreateTarea), new { id = tarea.IdTarea }, tarea);


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
                

                await _context.SaveChangesAsync();

                return Ok(new { success = true, message = "Tareas actualizada correctamente.", data = tarea }); ;
                
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
