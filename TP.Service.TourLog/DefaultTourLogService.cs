using AutoMapper;
using TP.Domain;

namespace TP.Service.TourLog;

using TourLog = Domain.TourLog;

public class DefaultTourLogService(TourComputedPropertiesCoordinator computedPropertiesCoordinator, IMapper mapper) : TourLogService
{
    public TourLog CreateTourLog(TourLogDTO tourLogDto, Tour tour)
    {
        TourLog newTourLog = mapper.Map<TourLog>(tourLogDto);
        newTourLog.Id = Guid.NewGuid();
        tour.TourLogs.Add(newTourLog);
        tour.UnprocessedLogsCounter++;

        if (computedPropertiesCoordinator.NeedToRecompute(tour)) computedPropertiesCoordinator.Recompute(tour);

        return newTourLog;
    }
}