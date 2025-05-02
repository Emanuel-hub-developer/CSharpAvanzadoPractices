using Microsoft.EntityFrameworkCore;
using TareaAPI.Common.Models;
using TareaAPI.Infrastructure.Entities;

namespace TareaAPI.Infrastructure.Data
{
    public class TareaAPIContext : DbContext
    {
        public TareaAPIContext(DbContextOptions options)
          : base(options)
        {

        }

        public DbSet<TareaEntity> Tareas { get; set; }
    }
}
