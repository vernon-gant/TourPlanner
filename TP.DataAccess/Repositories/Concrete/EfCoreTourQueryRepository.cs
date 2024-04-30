using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TP.Database;
using TP.Domain;

namespace TP.DataAccess.Repositories.Concrete;

public class EfCoreTourQueryRepository(AppDbContext dbContext, ILogger<EfCoreTourQueryRepository> logger) : TourQueryRepository
{
    public async ValueTask<List<Tour>> GetAllToursAsync()
    {
        try
        {
            return await dbContext.Tours.Include(tour => tour.TourLogs).ToListAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all tours");
            throw;
        }
    }

    public async ValueTask<Tour?> GetTourByIdAsync(Guid id)
    {
        try
        {
            return await dbContext.Tours.Include(tour => tour.TourLogs).FirstOrDefaultAsync(t => t.Id == id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving tour with ID: {TourId}", id);
            throw;
        }
    }

    public async ValueTask<List<Tour>> GetByIds(HashSet<Guid> tourIds, bool withTours)
    {
        try
        {
            if (withTours) return await dbContext.Tours.Where(tour => tourIds.Contains(tour.Id)).Include(tour => tour.TourLogs).ToListAsync();

            return await dbContext.Tours.Where(tour => tourIds.Contains(tour.Id)).ToListAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all tours");
            throw;
        }
    }

    public IQueryable<Tour> GetAllToursQueryable()
    {
        try
        {
            return dbContext.Tours;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving all tours");
            throw;
        }
    }

    public IQueryable<Tour> GetTourByIdQueryable(Guid id)
    {
        try
        {
            return dbContext.Tours.Where(t => t.Id == id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving tour with ID: {TourId}", id);
            throw;
        }
    }
}