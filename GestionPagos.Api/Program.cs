using GestionPagos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Detectar entorno
var isCloudRun = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("K_SERVICE"));
var connectionString = "";

if (isCloudRun)
{
    // Cloud Run: PostgreSQL con IP privada
    var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "10.70.0.5";
    var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "GestionPagos";
    var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "gp_app";
    var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
    
    connectionString = $"Host={dbHost};Database={dbName};Username={dbUser};Password={dbPassword};SSL Mode=Disable;";
    Console.WriteLine("[CLOUD RUN] Usando PostgreSQL en Cloud SQL");
}
else
{
    // Local: desde appsettings.json
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";
    Console.WriteLine("[LOCAL] Usando connection string de appsettings.json");
}

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Controllers, Swagger, CORS
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Migraciones automáticas en Cloud Run
if (isCloudRun)
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Console.WriteLine("[CLOUD RUN] Aplicando migraciones...");
        db.Database.Migrate();
        Console.WriteLine("[CLOUD RUN] Migraciones completadas");
    }
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors("AllowAll");
app.MapControllers();

// Health check
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.Run();
