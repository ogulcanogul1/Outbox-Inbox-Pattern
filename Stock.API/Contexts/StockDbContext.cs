using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Stock.API.Models;

namespace Stock.API.Contexts
{
    public class StockDbContext : DbContext
    {
        public StockDbContext(DbContextOptions options) : base(options) 
        {
            
        }

        public DbSet<Models.Stock> Stocks { get; set; }
        public DbSet<OrderInbox> OrderInboxes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderInbox>().HasKey(x => x.IdempotentToken);
            base.OnModelCreating(modelBuilder);
        }
    }
}
