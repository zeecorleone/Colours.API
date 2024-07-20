using Colours.API.Models;
using Microsoft.EntityFrameworkCore;

namespace Colours.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        
    }

    public DbSet<Colour> Colours { get; set; }
}
