using System.ComponentModel.DataAnnotations;

namespace TP.Report;

public class ReportRequest
{
    [Required]
    public string ReportType { get; set; } = string.Empty;

    [Required]
    public string Payload { get; set; } = string.Empty;
}