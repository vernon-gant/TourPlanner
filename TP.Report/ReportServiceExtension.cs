using Microsoft.Extensions.DependencyInjection;

namespace TP.Report;

public static class ReportServiceExtension
{
    public static void AddReporting(this IServiceCollection services)
    {
        services.AddScoped<ReportCoordinator, DefaultCoordinator>();
        services.AddScoped<FileHandler, FileStreamHandler>();
        services.AddScoped<DataProvisioner, EfCoreProvisioner>();
        services.AddScoped<ReportGenerator, PdfReportGenerator>();
    }
}