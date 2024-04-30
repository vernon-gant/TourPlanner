using Npoi.Mapper.Attributes;
using TP.Domain;
using TP.Utils;
using TransportType = System.Net.TransportType;

namespace TP.Export;

public class TourExportModel
{
    public int TourNumber { get; set; }

    public required string Description { get; set; }

    public required string Name { get; set; }

    public TransportType TransportType { get; set; }

    public required string Start { get; set; }

    public required Coordinates StartCoordinates { get; set; }

    public required string End { get; set; }

    public required Coordinates EndCoordinates { get; set; }

    public decimal DistanceMeters { get; set; }

    public TimeSpan EstimatedTime { get; set; }

    public Popularity? Popularity { get; set; }

    public string? ChildFriendliness { get; set; }

    [Ignore]
    public IList<TourLog> TourLogs { get; set; } = new List<TourLog>();
}