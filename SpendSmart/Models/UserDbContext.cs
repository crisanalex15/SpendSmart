using Microsoft.EntityFrameworkCore;
namespace SpendSmart.Models
{
    public class UserDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } // creeaza db pentru users

        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {

        }
    }
}
