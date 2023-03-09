using MAP_S_TestApp.Models.Domains;

namespace MAP_S_TestApp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }

    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
}
