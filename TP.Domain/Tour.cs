using System.ComponentModel.DataAnnotations.Schema;
using NpgsqlTypes;
using TP.Utils;

namespace TP.Domain;

public enum TransportType
{
    Foot = 100,
    Car = 200,
    Truck = 300,
    Bicycle = 400,
    Bike = 500
}

public enum Popularity
{
    NotPopular = 100,
    SlightlyPopular = 200,
    ModeratelyPopular = 300,
    Popular = 400,
    HighlyPopular = 500,
    ExtremelyPopular = 600
}

public class Tour
{
    public Guid Id { get; set; }

    public required string Description { get; set; }

    public required string Name { get; set; }

    public TransportType TransportType { get; set; }

    public required string Start { get; set; }

    public required Coordinates StartCoordinates { get; set; }

    public required string End { get; set; }

    public required Coordinates EndCoordinates { get; set; }

    public decimal DistanceMeters { get; set; }

    public TimeSpan EstimatedTime { get; set; }

    public string RouteGeometry { get; set; }

    public Popularity? Popularity { get; set; }

    public bool? ChildFriendliness { get; set; }

    public DateTime CreatedOn { get; set; }

    public int UnprocessedLogsCounter { get; set; }

    public IList<TourLog> TourLogs { get; set; } = new List<TourLog>();

    public const ushort MaxNameLength = 50;

    public const ushort MaxPointDescriptionLength = 50;

    [Column("FTX")]
    public NpgsqlTsVector SearchVector { get; } = null!;
}