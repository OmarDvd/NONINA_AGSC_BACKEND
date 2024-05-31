using NONINA_AGSC_APP_BACKEND.Models;
using Microsoft.EntityFrameworkCore;

namespace NONINA_AGSC_APP_BACKEND_API.Controllers
{
    public class CategoryAPIDbContext : DbContext
    {
        public CategoryAPIDbContext(DbContextOptions<CategoryAPIDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
    }
}
