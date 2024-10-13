using Colours.API.Data;
using Colours.API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddEntityFrameworkNpgsql()
    .AddDbContext<AppDbContext>(opt =>
    {
        opt.UseNpgsql(builder.Configuration.GetConnectionString("Default"));
    });

builder.Services.AddScoped<ICacheService, CacheService>();

var app = builder.Build();

//appy migrations automatically:
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }
    catch(Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error occurred while applying migrations.");
    }
}

var logFilePath = Path.Combine("/app/logs", "app.log");
var logDirectory = Path.GetDirectoryName(logFilePath);
if (!Directory.Exists(logDirectory))
    Directory.CreateDirectory(logDirectory!);

app.Use(async (context, next) =>
{
    if (!context.Request.Path.StartsWithSegments("/swagger"))
    {
        var logEntry = $"{DateTime.Now}: {context.Request.Method} {context.Request.Path}";
        await File.AppendAllTextAsync(logFilePath, logEntry + " \n\n");
    }
    await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
