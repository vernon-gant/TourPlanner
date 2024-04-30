using AutoMapper;

namespace TP.Service.Tour;

public class TourProfile : Profile
{
    public TourProfile()
    {
        CreateMap<TourDTO, Domain.Tour>();
        CreateMap<RouteInformation, Domain.Tour>()
            .ForMember(tour => tour.EstimatedTime, opt => opt.MapFrom(routeInformation => TimeSpan.FromSeconds((double)routeInformation.EstimatedTimeSeconds)));
    }
}