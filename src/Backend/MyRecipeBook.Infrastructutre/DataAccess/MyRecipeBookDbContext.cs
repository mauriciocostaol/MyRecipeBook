using Microsoft.EntityFrameworkCore;
using MyRecipeBook.Domain.Entities;

namespace MyRecipeBook.Infrastructutre.DataAccess;
public class MyRecipeBookDbContext: DbContext
{
    public MyRecipeBookDbContext(DbContextOptions options):base(options)
    {
        
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Recipe> Recipes { get; set; }
   

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyRecipeBookDbContext).Assembly);
    }
}
