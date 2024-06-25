using AutoMapper;
using TP.Domain;

namespace TP.Export;

public class ExportProfile : Profile
{
    public ExportProfile()
    {
        CreateMap<Tour, TourExportModel>()
           .ForMember(exportModel => exportModel.ChildFriendliness, opt => opt.MapFrom<BooleanToStringResolver>())
           .ForMember(exportModel => exportModel.StartLatitude, opt => opt.MapFrom(tour => tour.StartCoordinates.Latitude))
           .ForMember(exportModel => exportModel.StartLongitude, opt => opt.MapFrom(tour => tour.StartCoordinates.Longitude))
           .ForMember(exportModel => exportModel.EndLatitude, opt => opt.MapFrom(tour => tour.EndCoordinates.Latitude))
           .ForMember(exportModel => exportModel.EndLongitude, opt => opt.MapFrom(tour => tour.EndCoordinates.Longitude))
           .ForMember(exportModel => exportModel.EstimatedTime, opt => opt.MapFrom(tour => tour.EstimatedTime.Ticks))
           .ForMember(exportModel => exportModel.RouteGeometry, opt => opt.MapFrom(tour => tour.RouteGeometry));
        CreateMap<TourLog, TourLogExportModel>()
           .ForMember(exportModel => exportModel.TotalTime, opt => opt.MapFrom(tour => tour.TotalTime.Ticks));
    }
}