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

    public string Description { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public TransportType TransportType { get; set; }

    public int TransportTypeInt => (int)TransportType;

    public string Start { get; set; } = string.Empty;

    public string End { get; set; } = string.Empty;

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