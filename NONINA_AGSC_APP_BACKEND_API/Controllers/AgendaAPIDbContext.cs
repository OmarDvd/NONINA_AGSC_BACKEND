using NONINA_AGSC_APP_BACKEND.Models;
using Microsoft.EntityFrameworkCore;

namespace NONINA_AGSC_APP_BACKEND_API.Controllers
{
    public class AgendaAPIDbContext : DbContext
    {
        public AgendaAPIDbContext(DbContextOptions<AgendaAPIDbContext> options) : base(options)
        {
        }

        public DbSet<Agenda> Agendas { get; set; }
    }
}
