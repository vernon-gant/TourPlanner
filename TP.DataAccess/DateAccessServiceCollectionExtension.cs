using Microsoft.Extensions.DependencyInjection;

namespace TP.DataAccess;

public static class DateAccessServiceCollectionExtension
{
    public static void AddDataAccess(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<TourRepository, EfCoreTourRepository>();
    }
}