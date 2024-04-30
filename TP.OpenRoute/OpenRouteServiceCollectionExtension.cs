using Microsoft.Extensions.DependencyInjection;

namespace TP.OpenRoute;

public static class OpenRouteServiceCollectionExtension
{
    public static void AddOpenRoute(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<OpenRouteService, DefaultOpenRouteService>();
        serviceCollection.AddSingleton<OpenRouteValidator, DefaultOpenRouteValidator>();
    }
}