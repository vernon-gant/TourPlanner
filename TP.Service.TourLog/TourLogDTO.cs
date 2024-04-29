using TP.Domain;

namespace TP.Service.TourLog;

public class TourLogDTO
{
    public string UserName { get; set; } = string.Empty;

    public string Comment { get; set; } = string.Empty;

    public Difficulty Difficulty { get; set; }

    public decimal TotalDistanceMeters { get; set; }

    public TimeSpan TotalTime { get; set; }

    public short Rating { get; set; }
}