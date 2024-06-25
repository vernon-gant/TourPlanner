namespace TP.Export;

public static class ExportHeaders
{
    public static readonly List<string> TourHeaders =
    [
        "TourNumber", "Description", "Name", "TransportType", "Start", "StartLatitude", "StartLongitude", "End",
        "EndLatitude", "EndLongitude", "RouteGeometry", "DistanceMeters", "EstimatedTime", "Popularity", "ChildFriendliness"
    ];

    public static readonly List<string> TourLogHeaders = ["TourNumber", "Comment", "Difficulty", "TotalDistanceMeters", "TotalTime", "Rating"];
}