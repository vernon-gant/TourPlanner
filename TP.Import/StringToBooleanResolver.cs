using AutoMapper;
using TP.Domain;
using TP.Export;

namespace TP.Import;

public class StringToBooleanResolver : IValueResolver<TourExportModel, Tour, bool?>
{
    public bool? Resolve(TourExportModel source, Tour destination, bool? destMember, ResolutionContext context)
    {
        if (string.IsNullOrWhiteSpace(source.ChildFriendliness))
            return null;

        return source.ChildFriendliness.ToLower() switch
        {
            "true" => true,
            "false" => false,
            _ => null
        };
    }
}
