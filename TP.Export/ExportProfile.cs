using AutoMapper;
using TP.Domain;

namespace TP.Export;

public class ExportProfile : Profile
{
    public ExportProfile()
    {
        CreateMap<Tour, TourExportModel>()
            .ForMember(exportModel => exportModel.ChildFriendliness, opt => opt.MapFrom<BooleanToStringResolver>());
        CreateMap<TourLog, TourLogExportModel>();
    }
}