using Microsoft.EntityFrameworkCore;
using WebApplicationPronia.Entities;

namespace WebApplicationPronia.Contexts
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions option) : base(option) 
        {
            
        }
        public DbSet<InfoCard> InfoCards { get; set; }
    }
}
