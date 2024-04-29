using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace TP.Database;

public static class DatabaseSetupExtension
{
    public static async Task SetupDatabaseAsync(this IHost webHost)
    {
        using var scope = webHost.Services.CreateScope();
        var services = scope.ServiceProvider;

        var dbContext = services.GetRequiredService<AppDbContext>();
        var logger = services.GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
            if (!pendingMigrations.Any()) return;

            logger.LogInformation("Pending migrations found. Applying migrations to the database");
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Database migration completed successfully");
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occurred while migrating the database");
            throw;
        }
    }
}