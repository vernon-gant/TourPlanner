using TP.Report.ReportTypes;

namespace TP.Report;

public abstract class ReportGenerator
{
    protected string _filePath = string.Empty;

    public void SetFilePath(string filePath)
    {
        _filePath = filePath;
    }

    public abstract void Init();

    public abstract void Generate(SingleTourReport report);

    public abstract void Generate(TourSummaryReport report);
}