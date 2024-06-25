using Microsoft.Extensions.DependencyInjection;

namespace TP.Export;

public static class ExportServiceCollectionExtension
{
    public static void AddExport(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(typeof(ExportServiceCollectionExtension));
        serviceCollection.AddSingleton<TourMapper, DefaultTourMapper>();
        serviceCollection.AddSingleton<TourExporter, XlsxExporter>();
        serviceCollection.AddSingleton<TourExporter, CsvExporter>();
    }
}