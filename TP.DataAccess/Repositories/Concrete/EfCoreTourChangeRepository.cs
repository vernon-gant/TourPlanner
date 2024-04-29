using Microsoft.Extensions.Logging;
using TP.DataAccess.Repositories;
using TP.Database;
using TP.Domain;

namespace TP.DataAccess;

public class EfCoreTourChangeRepository(AppDbContext dbContext, ILogger<EfCoreTourChangeRepository> logger) : TourChangeRepository
{
    public async ValueTask<Tour> CreateTourAsync(Tour tour)
    {
        try
        {
            if (tour.Id == Guid.Empty) tour.Id = Guid.NewGuid();

            dbContext.Tours.Add(tour);
            await dbContext.SaveChangesAsync();
            return tour;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error adding tour");
            throw;
        }
    }

    public async ValueTask<Tour> UpdateTourAsync(Tour tour)
    {
        try
        {
            dbContext.Tours.Update(tour);
            await dbContext.SaveChangesAsync();
            return tour;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error updating tour with ID: {TourId}", tour.Id);
            throw;
        }
    }

    public async ValueTask DeleteTourAsync(Guid id)
    {
        try
        {
            var tour = await dbContext.Tours.FindAsync(id);
            if (tour == null) return;

            dbContext.Tours.Remove(tour);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting tour with ID: {TourId}", id);
            throw;
        }
    }
}