using TP.Domain;

namespace TP.Service.Tour;

public class TourDTO
{
    public string Description { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public TransportType TransportType { get; set; }

    public string Start { get; set; } = string.Empty;

    public string End { get; set; } = string.Empty;
}