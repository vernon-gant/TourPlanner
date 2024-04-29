using AutoMapper;

namespace TP.Service.TourLog;

public class TourLogProfile : Profile
{
    public TourLogProfile()
    {
        CreateMap<TourLogDTO, Domain.TourLog>();
    }
}