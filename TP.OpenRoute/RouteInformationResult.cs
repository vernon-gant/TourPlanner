using TP.Service.Tour;

namespace TP.OpenRoute;

public class RouteInformationResult
{
    public bool IsOk { get; set; }

    public RouteInformation? RouteInformation { get; set; }

    public string? ErrorMessage { get; set; }

    public static RouteInformationResult Ok(RouteInformation routeInformation) => new ()
    {
        IsOk = true,
        RouteInformation = routeInformation
    };

    public static RouteInformationResult Invalid(string errorMessage) => new()
    {
        IsOk = false,
        ErrorMessage = errorMessage
    };

}