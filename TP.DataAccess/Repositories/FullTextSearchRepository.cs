using TP.Domain;

namespace TP.DataAccess.Repositories;

public abstract class FullTextSearchRepository
{
    public abstract ValueTask<SearchStatus> SearchToursAsync(string searchTerm);

    public abstract ValueTask<SearchStatus> SearchTourLogsAsync(string searchTerm);

    public abstract SearchStatus TourSearchStatus { get; }

    public abstract SearchStatus TourLogSearchStatus { get; }

    public abstract List<Tour> FoundTours { get; }

    public abstract List<TourLog> FoundTourLogs { get; }
}

public interface SearchStatus;

public class Ok : SearchStatus;

public class NoSearchYet : SearchStatus;

public class DatabaseError : SearchStatus;