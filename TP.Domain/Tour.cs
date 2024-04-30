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
    NotPopular,
    SlightlyPopular,
    ModeratelyPopular,
    Popular,
    HighlyPopular,
    ExtremelyPopular
}

public class Tour
{
    public Guid Id { get; set; }

    public required string Description { get; set; }

    public required string Name { get; set; }

    public TransportType TransportType { get; set; }

    public int TransportTypeInt => (int)TransportType;

    public required string Start { get; set; }

    public required Coordinates StartCoordinates { get; set; }

    public required string End { get; set; }

    public required Coordinates EndCoordinates { get; set; }

    public decimal DistanceMeters { get; set; }

    public TimeSpan EstimatedTime { get; set; }

    public Popularity? Popularity { get; set; }

    public bool? ChildFriendliness { get; set; }

    public DateTime CreatedOn { get; set; }

    public int UnprocessedLogsCounter { get; set; }

    public IList<TourLog> TourLogs { get; set; } = new List<TourLog>();

    public const ushort MaxNameLength = 50;

    public const ushort MaxPointDescriptionLength = 50;
}