using TareaAPI.Infrastructure.Entities;

namespace TareaAPI.Factory
{
    public static class TareaFactory
    { 
        public static TareaEntity CrearTareaBasica(string description, DateTime dueDate, string status, string? additionalData = null)
        {
            return new TareaEntity
            {
                Description = description,
                DueDate = dueDate,
                Status = status,
                AdditionalData = additionalData
            };
        }

        public static TareaEntity CrearTareaUrgente(string description)
        {
            return new TareaEntity
            {
                Description = description,
                DueDate = DateTime.Now.AddDays(1),
                Status = "Pendiente",
                AdditionalData = "Prioridad alta"
            };
        }
    }
}
