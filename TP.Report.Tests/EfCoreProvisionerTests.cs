using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.EntityFrameworkCore;
using TP.Database;
using TP.Domain;
using TP.Report.ReportTypes;
using TP.Utils;

namespace TP.Report.Tests;

public class EfCoreProvisionerTests
{
    private readonly Mock<AppDbContext> _dbContextMock;
    private readonly EfCoreProvisioner _provisioner;
    private readonly Mock<ILogger<EfCoreProvisioner>> _loggerMock;

    public EfCoreProvisionerTests()
    {
        _dbContextMock = new Mock<AppDbContext>();
        _loggerMock = new Mock<ILogger<EfCoreProvisioner>>();
        _provisioner = new EfCoreProvisioner(_dbContextMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Provision_SingleTourReport_TourFound_ProvisionedOk()
    {
        // Arrange
        var tourId = Guid.NewGuid();
        var tours = new List<Tour>
        {
            new Tour
            {
                Id = tourId,
                Name = "Test Tour",
                Description = "Test Description",
                Start = "Start Location",
                End = "End Location",
                StartCoordinates = new Coordinates(1, 1),
                EndCoordinates = new Coordinates(2, 2),
                DistanceMeters = 1000,
                EstimatedTime = new TimeSpan(2, 0, 0),
                TransportType = TransportType.Bicycle,
                Popularity = Popularity.Popular,
                ChildFriendliness = true,
                TourLogs = new List<TourLog>
                {
                    new()
                    {
                        Comment = "Test Comment",
                        Difficulty = Difficulty.Normal,
                        TotalDistanceMeters = 1000,
                        TotalTime = new TimeSpan(1, 0, 0),
                        Rating = 5,
                        CreatedOn = new DateTime(2022, 1, 1, 12, 0, 0)
                    }
                }
            }
        };

        _dbContextMock.Setup(x => x.Tours).ReturnsDbSet(tours);

        var report = new SingleTourReport { TourId = tourId.ToString()};

        // Act
        var result = await _provisioner.Provision(report);

        // Assert
        Assert.IsType<ProvisionedOk>(result);
        Assert.Equal("Test Tour", report.TourName);
        Assert.Equal("Test Description", report.TourDescription);
        Assert.Equal("Start Location", report.StartLocation);
        Assert.Equal("End Location", report.EndLocation);
        Assert.Equal("1000 meters", report.Distance);
        Assert.Equal("00.02:00:00", report.EstimatedTime);
        Assert.Equal("Bicycle", report.TransportType);
        Assert.Equal("Popular", report.Popularity);
        Assert.Equal("Yes", report.ChildFriendly);

        var log = Assert.Single(report.TourLogs);
        Assert.Equal("Test Comment", log.Comment);
        Assert.Equal("Normal", log.Difficulty);
        Assert.Equal("1000 meters", log.Distance);
        Assert.Equal("00.01:00:00", log.TotalTime);
        Assert.Equal("5", log.Rating);
        Assert.Equal("2022-01-01 12:00:00", log.CreatedAt);
    }

    [Fact]
    public async Task Provision_TourSummaryReport_ToursAvailable_ProvisionedOk()
    {
        // Arrange
        var tours = new List<Tour>
        {
            new Tour
            {
                Id = Guid.NewGuid(),
                Name = "Test Tour",
                Description = "Test Description",
                Start = "Start Location",
                End = "End Location",
                StartCoordinates = new Coordinates(1, 1),
                EndCoordinates = new Coordinates(2, 2),
                TourLogs = new List<TourLog>
                {
                    new TourLog
                    {
                        TotalDistanceMeters = 1000,
                        TotalTime = new TimeSpan(1, 0, 0),
                        Rating = 5,
                        CreatedOn = DateTime.Now
                    },
                    new TourLog
                    {
                        TotalDistanceMeters = 2000,
                        TotalTime = new TimeSpan(2, 0, 0),
                        Rating = 7,
                        CreatedOn = DateTime.Now
                    }
                }
            }
        };

        _dbContextMock.Setup<DbSet<Tour>>(x => x.Tours).ReturnsDbSet(tours);

        var report = new TourSummaryReport();

        // Act
        var result = await _provisioner.Provision(report);

        // Assert
        Assert.IsType<ProvisionedOk>(result);
        Assert.Single(report.TourSummaries);
        var tourSummary = report.TourSummaries.First();
        Assert.Equal("Test Tour", tourSummary.TourName);
        Assert.Equal("1500,00 meters", tourSummary.AverageDistance);
        Assert.Equal("00.01:30:00", tourSummary.AverageTime);
        Assert.Equal("6,00", tourSummary.AverageRating);
    }

    [Fact]
    public async Task Provision_SingleTourReport_TourNotFound_ReturnsTourNotFound()
    {
        // Arrange
        _dbContextMock.Setup(x => x.Tours).ReturnsDbSet(new List<Tour>());

        var report = new SingleTourReport { TourId = Guid.NewGuid().ToString() };

        // Act
        var result = await _provisioner.Provision(report);

        // Assert
        Assert.IsType<TourNotFound>(result);
    }

    [Fact]
    public async Task Provision_TourSummaryReport_NoToursFound_ReturnsTourNotFound()
    {
        // Arrange
        _dbContextMock.Setup(x => x.Tours).ReturnsDbSet(new List<Tour>());

        var report = new TourSummaryReport();

        // Act
        var result = await _provisioner.Provision(report);

        // Assert
        Assert.IsType<ProvisionFailed>(result);
    }
}