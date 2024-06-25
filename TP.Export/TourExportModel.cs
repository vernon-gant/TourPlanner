using Npoi.Mapper.Attributes;
using TP.Domain;

namespace TP.Export;

public class TourExportModel
{
    public int TourNumber { get; set; }

    public required string Description { get; set; }

    public required string Name { get; set; }

    public TransportType TransportType { get; set; }

    public required string Start { get; set; }

    public required decimal StartLatitude { get; set; }

    public required decimal StartLongitude { get; set; }

    public required string End { get; set; }

    public required decimal EndLatitude { get; set; }

    public required decimal EndLongitude { get; set; }

    public required string RouteGeometry { get; set; }

    public decimal DistanceMeters { get; set; }

    public long EstimatedTime { get; set; }

    public Popularity? Popularity { get; set; }

    public string? ChildFriendliness { get; set; }

    [Ignore]
    public List<TourLog> TourLogs { get; set; } = new();
}