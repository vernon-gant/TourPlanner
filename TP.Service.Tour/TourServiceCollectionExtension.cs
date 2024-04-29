using Microsoft.Extensions.DependencyInjection;

namespace TP.Service.Tour;

public static class TourServiceCollectionExtension
{
    public static void AddTour(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(typeof(TourServiceCollectionExtension));
        serviceCollection.AddSingleton<OpenRouteValidator, DefaultOpenRouteValidator>();
    }
}