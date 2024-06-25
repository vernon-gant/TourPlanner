using TP.Report.ReportTypes;

namespace TP.Report;

public interface DataProvisioner
{
    ValueTask<ProvisionResult> Provision(SingleTourReport report);

    ValueTask<ProvisionResult> Provision(TourSummaryReport report);
}

public interface ProvisionResult;

public class ProvisionedOk : ProvisionResult;

public class TourNotFound : ProvisionResult;

public class ProvisionFailed : ProvisionResult;