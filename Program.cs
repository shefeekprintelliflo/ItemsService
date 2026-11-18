using ItemsService.Data;
using ItemsService.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidation(); // enforces [Required]/[MaxLength] on request bodies
var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' is not configured.");

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();

// Create the database schema on startup (fine for a simple service; use migrations for production).
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
}

var items = app.MapGroup("/api/items");

items.MapGet("/", async (AppDbContext db) =>
    await db.Items.AsNoTracking().OrderBy(i => i.Id).ToListAsync());

items.MapGet("/{id:int}", async (int id, AppDbContext db) =>
    await db.Items.FindAsync(id) is { } item ? Results.Ok(item) : Results.NotFound());

items.MapPost("/", async (Item input, AppDbContext db) =>
{
    var item = new Item { Name = input.Name, Description = input.Description };
    db.Items.Add(item);
    await db.SaveChangesAsync();
    return Results.Created($"/api/items/{item.Id}", item);
});

items.MapPut("/{id:int}", async (int id, Item input, AppDbContext db) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    item.Name = input.Name;
    item.Description = input.Description;
    await db.SaveChangesAsync();
    return Results.Ok(item);
});

items.MapDelete("/{id:int}", async (int id, AppDbContext db) =>
{
    var item = await db.Items.FindAsync(id);
    if (item is null) return Results.NotFound();

    db.Items.Remove(item);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();
