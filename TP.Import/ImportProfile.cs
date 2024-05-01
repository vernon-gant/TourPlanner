using AutoMapper;
using TP.Domain;
using TP.Export;
using TP.Utils;

namespace TP.Import;

public class ImportProfile : Profile
{
    public ImportProfile()
    {
        CreateMap<TourExportModel, Tour>()
            .ForMember(tour => tour.ChildFriendliness, opt => opt.MapFrom<StringToBooleanResolver>())
            .ForMember(tour => tour.StartCoordinates, opt => opt.MapFrom(exportModel => new Coordinates(exportModel.StartLatitude,exportModel.StartLongitude)))
            .ForMember(tour => tour.EndCoordinates, opt => opt.MapFrom(exportModel => new Coordinates(exportModel.EndLatitude, exportModel.EndLongitude)))
            .ForMember(tour => tour.EstimatedTime, opt => opt.MapFrom(exportModel => new TimeSpan(exportModel.EstimatedTime)));
        CreateMap<TourLogExportModel, TourLog>();
    }
}