namespace TP.Report;

public interface ReportGenerationResult;

public class GeneratedOk : ReportGenerationResult
{
    public required Stream Stream { get; set; }
}

public class GenerationFailed : ReportGenerationResult;

public interface ReportCreationResult;

public class CreatedOk : ReportCreationResult;

public class InvalidReportType : ReportCreationResult;

public class InvalidReportPayload : ReportCreationResult;