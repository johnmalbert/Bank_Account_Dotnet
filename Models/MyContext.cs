using Microsoft.EntityFrameworkCore;

namespace Bank_Accounts.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Transaction> Transactions {get;set;}
    }
}