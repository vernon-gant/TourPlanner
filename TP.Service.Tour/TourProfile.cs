using AutoMapper;

namespace TP.Service.Tour;

public class TourProfile : Profile
{
    public TourProfile()
    {
        CreateMap<TourDTO, Domain.Tour>();
    }
}