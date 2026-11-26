using Microsoft.EntityFrameworkCore;

namespace TeamLink.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    // Example table:
    // public DbSet<Product> Products => Set<Product>();
}