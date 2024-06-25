namespace TP.Report.ReportTypes;

public class TourSummaryReport : AbstractReport
{
    public List<TourSummary> TourSummaries { get; private set; } = new();

    public override async ValueTask<ProvisionResult> ProvisionData(DataProvisioner dataProvisioner)
    {
        return await dataProvisioner.Provision(this);
    }

    public override void GenerateReport(ReportGenerator reportGenerator)
    {
        reportGenerator.Generate(this);
    }
}

public class TourSummary
{
    public string TourId { get; set; }
    public string TourName { get; set; } = string.Empty;
    public string AverageDistance { get; set; } = string.Empty;
    public string AverageTime { get; set; } = string.Empty;
    public string AverageRating { get; set; } = string.Empty;
}