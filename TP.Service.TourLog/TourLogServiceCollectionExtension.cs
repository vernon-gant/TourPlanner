using Microsoft.Extensions.DependencyInjection;

namespace TP.Service.TourLog;

public static class TourLogServiceCollectionExtension
{
    public static void AddTourLog(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(typeof(TourLogServiceCollectionExtension));
        serviceCollection.AddSingleton<TourComputedPropertiesCoordinator, DefaultComputedPropertiesCoordinator>();
        serviceCollection.AddSingleton<TourLogService, DefaultTourLogService>();
    }
}