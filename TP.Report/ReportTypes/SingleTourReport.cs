namespace TP.Report.ReportTypes;

public class SingleTourReport : AbstractReport
{
    public string TourId { get; set; }

    public string TourName { get; set; }

    public string TourDescription { get; set; }

    public string StartLocation { get; set; }

    public string EndLocation { get; set; }

    public string Distance { get; set; }

    public string EstimatedTime { get; set; }

    public string TransportType { get; set; }

    public string Popularity { get; set; }

    public string ChildFriendly { get; set; }

    public List<TourLogReportModel> TourLogs { get; set; }
    public override async ValueTask<ProvisionResult> ProvisionData(DataProvisioner dataProvisioner)
    {
        return await dataProvisioner.Provision(this);
    }

    public override void GenerateReport(ReportGenerator reportGenerator)
    {
        reportGenerator.Generate(this);
    }
}

public class TourLogReportModel
{
    public string Comment { get; set; }

    public string Difficulty { get; set; }

    public string Distance { get; set; }

    public string TotalTime { get; set; }

    public string Rating { get; set; }

    public string CreatedAt { get; set; }
}