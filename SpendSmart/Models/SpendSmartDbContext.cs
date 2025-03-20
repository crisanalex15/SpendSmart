using Microsoft.EntityFrameworkCore;
namespace SpendSmart.Models
{
    public class SpendSmartDbContext : DbContext
    {
        public DbSet<Expense> Expenses { get; set; } // creeaza db pentru expenses

        public SpendSmartDbContext(DbContextOptions<SpendSmartDbContext> options) : base(options)
        {
            
        }
    }
}
