using Microsoft.EntityFrameworkCore;
using TaskManagerApplication.Infrastructure.Data;
using TaskManagerApplication.Infrastructure.Repositories;
using TaskManagerApplication.Application.Interfaces;
using TaskManagerApplication.Application.Services;
using TaskManagerApplication.Domain.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=Data/tasks.db"));

// DI
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();

// Serve static files (wwwroot)
// static files are enabled via UseStaticFiles()

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

// Default file
app.MapGet("/", () => Results.Redirect("/index.html"));

// Ensure database and tables exist (Code-First)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
