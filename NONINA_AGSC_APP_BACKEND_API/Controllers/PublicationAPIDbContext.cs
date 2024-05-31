using NONINA_AGSC_APP_BACKEND.Models;
using Microsoft.EntityFrameworkCore;

namespace NONINA_AGSC_APP_BACKEND_API.Controllers
{
    public class PublicationAPIDbContext : DbContext
    {
        public PublicationAPIDbContext(DbContextOptions<PublicationAPIDbContext> options) : base(options)
        {
        }

        public DbSet<Publication> Publications { get; set; }
    }
}
