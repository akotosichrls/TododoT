using Microsoft.EntityFrameworkCore;
using TodoMvcApp.Models;

public class AppDbContext:DbContext{
    public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
    {
    }

    public DbSet<TodoItem>? TodoItem { get; set; }
}