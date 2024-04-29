using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TP.Database;
using TP.Domain;

namespace TP.DataAccess;

public class EfCoreTourRepository(AppDbContext dbContext, ILogger<EfCoreTourRepository> logger) : TourRepository
{
    public async ValueTask<List<Tour>> GetAllToursAsync()
    {
        try
        {
            return await dbContext.Tours.ToListAsync();
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
            return await dbContext.Tours.FirstOrDefaultAsync(t => t.Id == id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving tour with ID: {TourId}", id);
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

    public async ValueTask<Tour> AddTourAsync(Tour tour)
    {
        try
        {
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