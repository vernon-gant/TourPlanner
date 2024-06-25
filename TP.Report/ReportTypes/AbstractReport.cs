namespace TP.Report.ReportTypes;

public abstract class AbstractReport
{
    public string CreatedAt => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

    public abstract ValueTask<ProvisionResult> ProvisionData(DataProvisioner dataProvisioner);

    public abstract void GenerateReport(ReportGenerator reportGenerator);
}