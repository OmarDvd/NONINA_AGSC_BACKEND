using NONINA_AGSC_APP_BACKEND.Models;
using Microsoft.EntityFrameworkCore;

namespace NONINA_AGSC_APP_BACKEND_API.Controllers
{
    public class UserAPIDbContext : DbContext
    {
        public UserAPIDbContext(DbContextOptions<UserAPIDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
