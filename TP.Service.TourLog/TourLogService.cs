using TP.Domain;

namespace TP.Service.TourLog;

using TourLog = Domain.TourLog;

public interface TourLogService
{
    TourLog CreateTourLog(TourLogDTO tourLogDto, Tour tour);
}