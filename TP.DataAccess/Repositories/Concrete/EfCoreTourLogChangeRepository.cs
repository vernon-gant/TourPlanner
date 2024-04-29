using Microsoft.Extensions.Logging;
using TP.Database;
using TP.Domain;

namespace TP.DataAccess.Repositories.Concrete;

public class EfCoreTourLogChangeRepository(AppDbContext dbContext, ILogger<EfCoreTourLogChangeRepository> logger)
    : TourLogChangeRepository
{
    public async ValueTask<TourLog> CreateTourLogAsync(TourLog tourLog)
    {
        try
        {
            if (tourLog.Id == Guid.Empty) tourLog.Id = Guid.NewGuid();

            await dbContext.TourLogs.AddAsync(tourLog);
            await dbContext.SaveChangesAsync();

            return tourLog;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding tour log");
            throw;
        }
    }

    public async ValueTask UpdateTourLogAsync(TourLog tourLog)
    {
        try
        {
            dbContext.TourLogs.Update(tourLog);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating tour log with ID: {LogId}", tourLog.Id);
            throw;
        }
    }

    public async ValueTask DeleteTourLogAsync(Guid id)
    {
        try
        {
            TourLog? tourLog = await dbContext.TourLogs.FindAsync(id);

            if (tourLog == null) return;

            dbContext.TourLogs.Remove(tourLog);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting tour log with ID: {LogId}", id);
            throw;
        }
    }
}