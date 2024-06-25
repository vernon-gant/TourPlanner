using TP.Domain;

namespace TP.Service.TourLog;

public class DefaultComputedPropertiesCoordinator : TourComputedPropertiesCoordinator
{
    private const int UnprocessedTourLogsThreshold = 10;

    private readonly List<(int Threshold, Popularity Level)> popularityLevels =
    [
        (200, Popularity.ExtremelyPopular),
        (100, Popularity.HighlyPopular),
        (50, Popularity.Popular),
        (25, Popularity.ModeratelyPopular),
        (10, Popularity.SlightlyPopular),
        (0, Popularity.NotPopular)
    ];
    private const double HighRatingThreshold = 8;

    private const int EasyDifficultyThreshold = 300;
    private const double MaximumChildFriendlyDurationMinutes = 120;
    private const decimal MaximumChildFriendlyDistanceKilometers = 10;

    public bool NeedToRecompute(Tour tour) => tour.UnprocessedLogsCounter == UnprocessedTourLogsThreshold;

    public void Recompute(Tour tour)
    {
        int logCount = tour.TourLogs.Count;
        double averageRating = tour.TourLogs.Any() ? tour.TourLogs.Average(log => log.Rating) : 0;

        tour.Popularity = popularityLevels.First(level => logCount >= level.Threshold).Level;

        if (tour.Popularity >= Popularity.HighlyPopular && averageRating > HighRatingThreshold) tour.Popularity = Popularity.ExtremelyPopular;

        ComputeChildFriendliness(tour);
        tour.UnprocessedLogsCounter = 0;
    }

    private void ComputeChildFriendliness(Tour tour)
    {
        if (!tour.TourLogs.Any())
        {
            tour.ChildFriendliness = false;
            return;
        }

        double averageDifficulty = tour.TourLogs.Average(log => (int) log.Difficulty);
        double averageDurationMinutes = tour.TourLogs.Average(log => log.TotalTime.TotalMinutes);
        decimal averageTotalDistanceMeters = tour.TourLogs.Average(log => log.TotalDistanceMeters);

        tour.ChildFriendliness = averageDifficulty <= EasyDifficultyThreshold && averageDurationMinutes < MaximumChildFriendlyDurationMinutes && averageTotalDistanceMeters <= MaximumChildFriendlyDistanceKilometers;
    }
}