using NONINA_AGSC_APP_BACKEND.Models;
using Microsoft.EntityFrameworkCore;

namespace NONINA_AGSC_APP_BACKEND_API.Controllers
{
    public class EventoAPIDbContext : DbContext
    {
        public EventoAPIDbContext(DbContextOptions<EventoAPIDbContext> options) : base(options)
        {
        }

        public DbSet<Evento> Eventos { get; set; }
    }
}
