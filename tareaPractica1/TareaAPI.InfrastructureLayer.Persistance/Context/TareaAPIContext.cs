using Microsoft.EntityFrameworkCore;
using TareaAPI.Common.Models;
using TareaAPI.Core.DomainLayer.Models;
using TareaAPI.Infrastructure.Entities;
using TareaAPI.InfrastructureLayer.Persistance.Utilities;


namespace TareaAPI.Infrastructure.Data
{
    public class TareaAPIContext : DbContext
    {
        public TareaAPIContext(DbContextOptions options)
          : base(options)
        {

        }

        public DbSet<TareaEntity> Tareas { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<RecordRefreshTokens> RecordRefreshTokens { get; set; }
    }
}
