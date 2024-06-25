using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TP.Database;
using TP.Domain;

namespace TP.DataAccess.Repositories.Concrete;

public class EfCoreFullTextSearchRepository(AppDbContext dbContext, ILogger<EfCoreFullTextSearchRepository> logger) : FullTextSearchRepository
{
    private SearchStatus tourSearchStatus = new NoSearchYet();

    private SearchStatus tourLogSearchStatus = new NoSearchYet();

    private List<Tour> foundTours = new();

    private List<TourLog> foundTourLogs = new();

    public override async ValueTask<SearchStatus> SearchToursAsync(string searchTerm)
    {
        try
        {
            foundTours = await dbContext.Tours.Where(tour => tour.SearchVector.Matches(EF.Functions.ToTsQuery("english", searchTerm))).ToListAsync();
            tourSearchStatus = new Ok();
            return tourSearchStatus;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while searching tours");
            tourSearchStatus = new DatabaseError();
            return tourSearchStatus;
        }
    }

    public override async ValueTask<SearchStatus> SearchTourLogsAsync(string searchTerm)
    {
        try
        {
            foundTourLogs = await dbContext.TourLogs.Where(tourLog => tourLog.SearchVector.Matches(EF.Functions.PlainToTsQuery("english", searchTerm))).Include(log => log.Tour).ToListAsync();
            tourLogSearchStatus = new Ok();
            return tourLogSearchStatus;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while searching tour logs");
            tourLogSearchStatus = new DatabaseError();
            return tourLogSearchStatus;
        }
    }

    public override SearchStatus TourSearchStatus => tourSearchStatus;

    public override SearchStatus TourLogSearchStatus => tourLogSearchStatus;

    public override List<Tour> FoundTours => tourSearchStatus is Ok ? foundTours : throw new InvalidOperationException();

    public override List<TourLog> FoundTourLogs => tourLogSearchStatus is Ok ? foundTourLogs : throw new InvalidOperationException();
}