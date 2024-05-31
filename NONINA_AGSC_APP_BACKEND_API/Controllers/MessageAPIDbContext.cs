using NONINA_AGSC_APP_BACKEND.Models;
using Microsoft.EntityFrameworkCore;

namespace NONINA_AGSC_APP_BACKEND_API.Controllers
{
    public class MessageAPIDbContext : DbContext
    {
        public MessageAPIDbContext(DbContextOptions<MessageAPIDbContext> options) : base(options)
        {
        }

        public DbSet<Message> Messages { get; set; }
    }
}
