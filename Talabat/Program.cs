using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Talabat.Core.Entities.Identity;
using Talabat.Data;
using Talabat.Extensions;
using Talabat.Middlewares;
using Talabat.Repository.Data;
using Talabat.Repository.Identity;


var builder = WebApplication.CreateBuilder(args);

// Add services to the Dependency Injection Container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();

// Configure the main database connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

// Configure the Identity database connection
var IdentityConnection = builder.Configuration.GetConnectionString("IdentityConnection");
builder.Services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(IdentityConnection));

// Configure Redis connection
builder.Services.AddSingleton<IConnectionMultiplexer>(S =>
{
    var configurationString = builder.Configuration.GetConnectionString("ConnectionString");

    if (string.IsNullOrWhiteSpace(configurationString))
    {
        throw new InvalidOperationException("Redis connection string is not configured.");
    }
    var configuration = ConfigurationOptions.Parse(configurationString, true);

    return ConnectionMultiplexer.Connect(configuration);
}
);

builder.Services.AddApplicationServices();
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();

#region Database Migration and Seeding

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    var logger = loggerFactory.CreateLogger<Program>();

    try
    {
        // Migrate the main database
        var context = services.GetRequiredService<ApplicationDbContext>();
        logger.LogInformation("Starting database migration");
        await context.Database.MigrateAsync();
        await ApplicationDbContextSeed.SeedAsync(context, loggerFactory);

        var identityContext = services.GetRequiredService<AppIdentityDbContext>();
        await identityContext.Database.MigrateAsync();
        // Seed initial users
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        await AppIdentityDbContextSeed.SeedUserAsync(userManager);

        logger.LogInformation("Database migrated successfully");
    }
    catch (Exception ex)
    {
        var loggers = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred during database migration");
    }
}

#endregion

#region Configure the HTTP Request Pipeline

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

#endregion

await app.RunAsync();