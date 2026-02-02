using GestionPagos.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GestionPagos.Api.Services;

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

// Agregar servicio JWT
builder.Services.AddScoped<JwtService>();

// Configurar autenticación JWT
var jwtKey = builder.Configuration["Jwt:Key"] ?? "SuperSecretKeyForDevelopment12345678901234567890";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "GestionPagosApi";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "GestionPagosClient";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

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
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Health check
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.Run();
