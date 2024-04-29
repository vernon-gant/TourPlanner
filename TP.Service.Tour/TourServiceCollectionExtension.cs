using Microsoft.Extensions.DependencyInjection;
using TP.Domain;

namespace TP.Service.Tour;

public static class TourServiceCollectionExtension
{
    public static void AddTour(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(typeof(TourServiceCollectionExtension));
        serviceCollection.AddSingleton<OpenRouteValidator, DefaultOpenRouteValidator>();
        serviceCollection.AddSingleton<Dictionary<TransportType, string>>(_ => new Dictionary<TransportType, string>
        {
            { TransportType.Foot, "foot-walking" },
            { TransportType.Car, "driving-car" },
            { TransportType.Truck, "driving-hgv" },
            { TransportType.Bicycle, "cycling-regular" },
            { TransportType.Bike, "cycling-electric" }
        });
    }
}