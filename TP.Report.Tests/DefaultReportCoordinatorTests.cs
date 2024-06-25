using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;
using TP.Report.ReportTypes;

namespace TP.Report.Tests;

public class DefaultCoordinatorTests
{
    private readonly Mock<FileHandler> _fileHandlerMock;
    private readonly Mock<DataProvisioner> _dataProvisionerMock;
    private readonly Mock<ReportGenerator> _reportGeneratorMock;
    private readonly Mock<ILogger<DefaultCoordinator>> _loggerMock;
    private readonly DefaultCoordinator _coordinator;

    public DefaultCoordinatorTests()
    {
        _fileHandlerMock = new Mock<FileHandler>();
        _dataProvisionerMock = new Mock<DataProvisioner>();
        _reportGeneratorMock = new Mock<ReportGenerator>();
        _loggerMock = new Mock<ILogger<DefaultCoordinator>>();
        _coordinator = new DefaultCoordinator(_fileHandlerMock.Object, _dataProvisionerMock.Object, _reportGeneratorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public void CreateReportFromRequest_ValidRequest_ReturnsCreatedOk()
    {
        // Arrange
        var request = new ReportRequest
                      {
                          ReportType = "SingleTourReport",
                          Payload = JsonSerializer.Serialize(new { TourId = "1", TourName = "Test Tour" })
                      };

        // Act
        var result = _coordinator.CreateReportFromRequest(request);

        // Assert
        Assert.IsType<CreatedOk>(result);
    }

    [Fact]
    public void CreateReportFromRequest_InvalidReportType_ReturnsInvalidReportType()
    {
        // Arrange
        var request = new ReportRequest
                      {
                          ReportType = "InvalidReportType",
                          Payload = "{}"
                      };

        // Act
        var result = _coordinator.CreateReportFromRequest(request);

        // Assert
        Assert.IsType<InvalidReportType>(result);
    }

    [Fact]
    public void CreateReportFromRequest_InvalidPayload_ReturnsInvalidReportPayload()
    {
        // Arrange
        var request = new ReportRequest
                      {
                          ReportType = "SingleTourReport",
                          Payload = "invalid json"
                      };

        // Act
        var result = _coordinator.CreateReportFromRequest(request);

        // Assert
        Assert.IsType<InvalidReportPayload>(result);
    }

    [Fact]
    public async Task GenerateReport_ValidReport_GeneratesReportSuccessfully()
    {
        // Arrange
        var report = new SingleTourReport { TourId = "1", TourName = "Test Tour" };
        _coordinator.CreateReportFromRequest(new ReportRequest { ReportType = "SingleTourReport", Payload = JsonSerializer.Serialize(report) });

        _fileHandlerMock.Setup(fh => fh.GenerateFile());
        _fileHandlerMock.Setup(fh => fh.GetFileStream()).Returns(new MemoryStream());

        _dataProvisionerMock.Setup(dp => dp.Provision(It.IsAny<SingleTourReport>())).ReturnsAsync(new ProvisionedOk());

        // Act
        var result = await _coordinator.GenerateReport();

        // Assert
        Assert.IsType<GeneratedOk>(result);
        _fileHandlerMock.Verify(fh => fh.GenerateFile(), Times.Once);
        _reportGeneratorMock.Verify(rg => rg.Init(), Times.Once);
        _reportGeneratorMock.Verify(rg => rg.Generate(It.IsAny<SingleTourReport>()), Times.Once);
    }

    [Fact]
    public async Task GenerateReport_ProvisionFailed_ReturnsGenerationFailed()
    {
        // Arrange
        var report = new SingleTourReport { TourId = "1", TourName = "Test Tour" };
        _coordinator.CreateReportFromRequest(new ReportRequest { ReportType = "SingleTourReport", Payload = JsonSerializer.Serialize(report) });

        _fileHandlerMock.Setup(fh => fh.GenerateFile()).Verifiable();
        _fileHandlerMock.Setup(fh => fh.GetFileStream()).Returns(new MemoryStream());

        _dataProvisionerMock.Setup(dp => dp.Provision(It.IsAny<SingleTourReport>())).ReturnsAsync(new ProvisionFailed());

        // Act
        var result = await _coordinator.GenerateReport();

        // Assert
        Assert.IsType<GenerationFailed>(result);
    }

}