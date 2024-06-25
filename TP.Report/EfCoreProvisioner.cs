using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TP.Database;
using TP.Domain;
using TP.Report.ReportTypes;

namespace TP.Report;

public class EfCoreProvisioner(AppDbContext dbContext, ILogger<EfCoreProvisioner> logger) : DataProvisioner
{
    public async ValueTask<ProvisionResult> Provision(SingleTourReport report)
    {
        try
        {
            Tour? tour = await dbContext.Tours.Include(t => t.TourLogs).FirstOrDefaultAsync(t => t.Id.ToString() == report.TourId);

            if (tour == null) return new TourNotFound();

            report.TourName = tour.Name;
            report.TourDescription = tour.Description;
            report.StartLocation = tour.Start;
            report.EndLocation = tour.End;
            report.Distance = $"{tour.DistanceMeters} meters";
            report.EstimatedTime = tour.EstimatedTime.ToString(@"dd\.hh\:mm\:ss");
            report.TransportType = tour.TransportType.ToString();
            report.Popularity = tour.Popularity.HasValue ? tour.Popularity.Value.ToString() : "Not enough data";
            report.ChildFriendly = tour.ChildFriendliness.HasValue ? (tour.ChildFriendliness.Value ? "Yes" : "No") : "Not enough data";

            report.TourLogs = tour.TourLogs.Select(log => new TourLogReportModel
                                                      {
                                                          Comment = log.Comment,
                                                          Difficulty = log.Difficulty.ToString(),
                                                          Distance = $"{log.TotalDistanceMeters} meters",
                                                          TotalTime = log.TotalTime.ToString(@"dd\.hh\:mm\:ss"),
                                                          Rating = log.Rating.ToString(),
                                                          CreatedAt = log.CreatedOn.ToString("yyyy-MM-dd HH:mm:ss")
                                                      }).ToList();
            return new ProvisionedOk();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to provision data for SingleTourReport");
            return new ProvisionFailed();
        }
    }

    public async ValueTask<ProvisionResult> Provision(TourSummaryReport report)
    {
        try
        {
            List<Tour> tours = await dbContext.Tours.Include(t => t.TourLogs).ToListAsync();

            if (tours.Count == 0) return new ProvisionFailed();

            foreach (Tour tour in tours)
            {
                if (tour.TourLogs.Count == 0) continue;

                decimal averageDistance = tour.TourLogs.Average(log => log.TotalDistanceMeters);
                TimeSpan averageTime = new TimeSpan((long)tour.TourLogs.Average(log => log.TotalTime.Ticks));
                double averageRating = tour.TourLogs.Average(log => log.Rating);

                report.TourSummaries.Add(new TourSummary
                                         {
                                             TourId = tour.Id.ToString(),
                                             TourName = tour.Name,
                                             AverageDistance = $"{averageDistance:F2} meters",
                                             AverageTime = averageTime.ToString(@"dd\.hh\:mm\:ss"),
                                             AverageRating = averageRating.ToString("F2")
                                         });
            }
            return new ProvisionedOk();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to provision data for TourSummaryReport");
            return new ProvisionFailed();
        }
    }
}