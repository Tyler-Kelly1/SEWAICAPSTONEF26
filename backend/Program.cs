using Backend.Data;
using Backend.Engines;
using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Load .env configuration and inject dynamically
var envVars = EnvConfigurationHelper.LoadEnv(builder.Environment.ContentRootPath);
if (envVars.Count > 0)
{
    builder.Configuration.AddInMemoryCollection(envVars);
}

// Add services to the container with injected connection string
var connectionString = EnvConfigurationHelper.GetDatabaseConnectionString(builder.Configuration);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configure ASP.NET Core Identity with ApplicationUser
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Configure 24-hour cookie authentication with sliding expiration
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "OverloadAuth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
    options.SlidingExpiration = true;
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
});

builder.Services.AddScoped<IAuthEngine, AuthEngine>();
builder.Services.AddScoped<IWorkoutSessionEngine, WorkoutSessionEngine>();
builder.Services.AddScoped<IOverloadEngine, OverloadEngine>();
builder.Services.AddScoped<IUserEngine, UserEngine>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS for Vue Vite frontend with credential support
var allowedOrigins = new List<string> { "http://localhost:5173", "http://127.0.0.1:5173", "http://localhost", "http://localhost:80" };
var configuredOrigins = builder.Configuration["ALLOWED_ORIGINS"] ?? Environment.GetEnvironmentVariable("ALLOWED_ORIGINS");
if (!string.IsNullOrWhiteSpace(configuredOrigins))
{
    allowedOrigins.AddRange(configuredOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins.Distinct().ToArray())
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Auto-apply EF Core Migrations or ensure database creation at startup and seed initial fake data
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        // Fallback to EnsureCreated if migrations fail or DB is being provisioned dynamically
        app.Logger.LogWarning(ex, "EF Core Migration failed on startup. Attempting EnsureCreated...");
        dbContext.Database.EnsureCreated();
    }

    try
    {
        DbInitializer.SeedAsync(dbContext).GetAwaiter().GetResult();
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Failed to seed database.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

// Health check endpoint for container orchestrators and load balancers
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow
}));

app.MapControllers();

app.Run();
