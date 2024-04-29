using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TP.Database;
using TP.Domain;

namespace TP.DataAccess.Repositories.Concrete;

public class EfCoreTourLogQueryRepository(AppDbContext dbContext, ILogger<EfCoreTourLogQueryRepository> logger) : TourLogQueryRepository
{
    public async ValueTask<List<TourLog>> GetAllTourLogsAsync()
    {
        try
        {
            return await dbContext.TourLogs.Include(tourLog => tourLog.Tour).ToListAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all tour logs");
            throw;
        }
    }

    public async ValueTask<TourLog?> GetTourLogByIdAsync(Guid tourLogId)
    {
        try
        {
            return await dbContext.TourLogs.FirstOrDefaultAsync(log => log.Id == tourLogId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving tour log with ID: {LogId}", tourLogId);
            throw;
        }
    }

    public IQueryable<TourLog> GetAllTourLogsQueryable()
    {
        try
        {
            return dbContext.TourLogs;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all tour logs");
            throw;
        }
    }

    public IQueryable<TourLog> GetTourLogByIdQueryable(Guid tourLogId)
    {
        try
        {
            return dbContext.TourLogs.Where(log => log.Id == tourLogId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving tour log with ID: {LogId}", tourLogId);
            throw;
        }
    }
}