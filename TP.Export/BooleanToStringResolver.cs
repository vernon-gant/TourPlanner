using AutoMapper;
using TP.Domain;

namespace TP.Export;

public class BooleanToStringResolver : IValueResolver<Tour, TourExportModel, string?>
{
    public string? Resolve(Tour source, TourExportModel destination, string? destMember, ResolutionContext context)
    {
        if (source.ChildFriendliness == null) return null;

        return source.ChildFriendliness.Value ? "true" : "false";
    }
}