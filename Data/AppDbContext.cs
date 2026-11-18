using ItemsService.Models;
using Microsoft.EntityFrameworkCore;

namespace ItemsService.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Item> Items => Set<Item>();
}
