namespace TP.Domain;

public enum TransportType
{
    Foot = 100,
    Car = 200,
    Truck = 300,
    Bicycle = 400,
    Bike = 500
}

public class Tour
{
    public Guid Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public TransportType TransportType { get; set; }

    public int TransportTypeInt => (int)TransportType;

    public string StartDescription { get; set; } = string.Empty;

    public string EndDescription { get; set; } = string.Empty;

    public decimal Distance { get; set; }

    public TimeSpan EstimatedTime { get; set; }

    public int? Popularity { get; set; }

    public bool? ChildFriendliness { get; set; }

    public DateTime CreatedOn { get; set; }

    public IList<TourLog> TourLogs { get; set; } = new List<TourLog>();

    public const ushort MaxNameLength = 50;

    public const ushort MaxPointDescriptionLength = 50;
}