using Microsoft.EntityFrameworkCore;
using WebApiTestBook.Models;

namespace WebApiTestBook.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Order> Orders => Set<Order>();
    }
}
