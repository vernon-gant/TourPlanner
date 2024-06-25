using System.Text.Json;
using Microsoft.Extensions.Logging;
using TP.Report.ReportTypes;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TP.Report;

public interface ReportCoordinator
{
    ReportCreationResult CreateReportFromRequest(ReportRequest request);

    ValueTask<ReportGenerationResult> GenerateReport();

    public ReportGenerationResult ReportGenerationResult { get; }

    public ReportCreationResult ReportCreationResult { get; }
}

public class DefaultCoordinator(FileHandler fileHandler, DataProvisioner dataProvisioner, ReportGenerator reportGenerator, ILogger<DefaultCoordinator> logger) : ReportCoordinator
{
    private ReportGenerationResult _reportGenerationResult = null!;

    private ReportCreationResult _reportCreationResult = null!;

    private AbstractReport _report = null!;
    public ReportCreationResult CreateReportFromRequest(ReportRequest request)
    {
        try
        {
            Type? reportType = Type.GetType($"TP.Report.ReportTypes.{request.ReportType}");

            if (reportType is null) return _reportCreationResult = new InvalidReportType();

            object? report = JsonSerializer.Deserialize(request.Payload, reportType, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (report is null) return _reportCreationResult = new InvalidReportPayload();

            _report = (AbstractReport)report;

            return _reportCreationResult = new CreatedOk();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error creating report");
            return _reportCreationResult = new InvalidReportPayload();
        }
    }

    public async ValueTask<ReportGenerationResult> GenerateReport()
    {
        try
        {
            ProvisionResult provisionResult = await _report.ProvisionData(dataProvisioner);

            if (provisionResult is not ProvisionedOk) return _reportGenerationResult = new GenerationFailed();

            fileHandler.GenerateFile();
            reportGenerator.SetFilePath(fileHandler.FilePath);
            reportGenerator.Init();
            _report.GenerateReport(reportGenerator);
            _reportGenerationResult = new GeneratedOk { Stream = fileHandler.GetFileStream() };
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error generating report");
            _reportGenerationResult = new GenerationFailed();
        }
        return _reportGenerationResult;
    }



    public ReportGenerationResult ReportGenerationResult => _reportGenerationResult;

    public ReportCreationResult ReportCreationResult => _reportCreationResult;
}