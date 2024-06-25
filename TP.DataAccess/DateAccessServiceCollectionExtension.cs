using Microsoft.Extensions.DependencyInjection;
using TP.DataAccess.Repositories;
using TP.DataAccess.Repositories.Concrete;

namespace TP.DataAccess;

public static class DateAccessServiceCollectionExtension
{
    public static void AddDataAccess(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<TourQueryRepository, EfCoreTourQueryRepository>();
        serviceCollection.AddScoped<TourChangeRepository, EfCoreTourChangeRepository>();
        serviceCollection.AddScoped<TourLogQueryRepository, EfCoreTourLogQueryRepository>();
        serviceCollection.AddScoped<TourLogChangeRepository, EfCoreTourLogChangeRepository>();
        serviceCollection.AddScoped<FullTextSearchRepository, EfCoreFullTextSearchRepository>();
    }
}