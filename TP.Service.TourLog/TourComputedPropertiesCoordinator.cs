using TP.Domain;

namespace TP.Service.TourLog;

public interface TourComputedPropertiesCoordinator
{
    bool NeedToRecompute(Tour tour);

    void Recompute(Tour tour);
}