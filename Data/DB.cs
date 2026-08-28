using Microsoft.EntityFrameworkCore;
using webasp.Models;
namespace webasp.Data
{
    public class DB : DbContext
    {
        public DB(DbContextOptions<DB> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }

        
    }
}