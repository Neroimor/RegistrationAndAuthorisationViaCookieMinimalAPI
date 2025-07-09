using Microsoft.EntityFrameworkCore;
using RegistrationAndAuthorisationViaCookieMinimalAPI.DTO;

namespace RegistrationAndAuthorisationViaCookieMinimalAPI.DataBase
{
    public class AppDBContext : DbContext
    {

        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
            
        }
        public DbSet<User> Users_Data { get; set; }

    }
}
