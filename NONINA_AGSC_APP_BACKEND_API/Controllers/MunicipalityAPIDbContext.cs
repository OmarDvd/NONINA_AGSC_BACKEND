using NONINA_AGSC_APP_BACKEND.Models;
using Microsoft.EntityFrameworkCore;

namespace NONINA_AGSC_APP_BACKEND_API.Controllers
{
    public class MunicipalityAPIDbContext : DbContext
    {
        public MunicipalityAPIDbContext(DbContextOptions<MunicipalityAPIDbContext> options) : base(options)
        {
        }

        public DbSet<Municipality> Municipalities { get; set; }
    }
}
