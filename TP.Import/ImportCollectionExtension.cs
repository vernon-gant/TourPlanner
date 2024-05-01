using Microsoft.Extensions.DependencyInjection;

namespace TP.Import;

public static class ImportCollectionExtension
{
    public static void AddImport(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAutoMapper(typeof(ImportCollectionExtension));
        serviceCollection.AddSingleton<TourImporter, XlsxImporter>();
    }
}