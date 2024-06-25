using Microsoft.Extensions.Logging;
using TP.Database;
using TP.Domain;

namespace TP.DataAccess.Repositories.Concrete;

public class EfCoreTourLogsLoader(AppDbContext dbContext, ILogger<EfCoreTourLogsLoader> logger) : TourLoader
{
    public async ValueTask LoadTourLogsAsync(Tour tour)
    {
        try
        {
            await dbContext.Entry(tour)
                                  .Collection(t => t.TourLogs)
                                  .LoadAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error during loading tour logs");
            throw;
        }
    }

    public async ValueTask LoadTourLogsAsync(List<Tour> tours)
    {
        try
        {
            foreach (var tour in tours)
            {
                await LoadTourLogsAsync(tour);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error during loading tour logs");
            throw;
        }
    }
}