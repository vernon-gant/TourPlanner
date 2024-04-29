using AutoMapper;

namespace TP.Service.Tour;

using Tour = Domain.Tour;

public interface TourService
{
    Tour CreateTour(TourDTO tourDto, RouteInformation routeInformation);
}

public class DefaultTourService(IMapper mapper) : TourService
{
    public Tour CreateTour(TourDTO tourDto, RouteInformation routeInformation)
    {
        Tour newTour = mapper.Map<Tour>(tourDto);
        newTour.Distance = routeInformation.DistanceM;
        newTour.EstimatedTime = TimeSpan.FromSeconds((double)routeInformation.EstimatedDurationS);
        return newTour;
    }
}