using CSV_Parser;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
string? connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(connection));
var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();


app.MapGet("/", (ApplicationContext db) => db.Data.ToList());
app.MapGet("/api/records", async (ApplicationContext db) => await db.Data.ToListAsync());
app.MapGet("/api/records/{id:int}", async (int id, ApplicationContext db) =>
{
    CSVData? data = await db.Data.FirstOrDefaultAsync(u => u.ID == id);
    if (data == null) return Results.NotFound(new { message = "Немає такого запису" });
    return Results.Json(data);
});

app.Run();
