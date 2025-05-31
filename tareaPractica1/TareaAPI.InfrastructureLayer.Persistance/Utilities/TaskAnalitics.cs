using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TareaAPI.Infrastructure.Entities;

namespace TareaAPI.InfrastructureLayer.Persistance.Utilities
{
    public static class TaskAnalitics
    {
        private static readonly Dictionary<string, double> _cache = new();

        public static double CalcularPorcentajeCompletado(List<TareaEntity> tareas)
        {
            if (tareas == null || !tareas.Any()) return 0;

            string key = string.Join(",", tareas
                .OrderBy(t => t.IdTarea)
                .Select(t => $"{t.IdTarea}-{t.Status}"));

            if (_cache.TryGetValue(key, out var cached))
                return cached;

            int completadas = tareas.Count(t => t.Status == "Completada");
            double porcentaje = (double)completadas / tareas.Count * 100;

            _cache[key] = porcentaje;

            return porcentaje;
        }
    }
}
