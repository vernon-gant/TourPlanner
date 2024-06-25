using System.Net;
using Microsoft.Extensions.Logging;
using Moq;
using TP.Domain;
using TP.Service.Tour;
using TP.Utils;

namespace TP.OpenRoute.Tests;

public class DefaultOpenRouteServiceTests
{
    private readonly Mock<OpenRouteClient> _openRouteClientMock;
    private readonly Mock<OpenRouteValidator> _openRouteValidatorMock;
    private readonly Mock<ILogger<DefaultOpenRouteService>> _loggerMock;
    private readonly Dictionary<TransportType, string> _typeToProfileMap;
    private readonly DefaultOpenRouteService _service;

    public DefaultOpenRouteServiceTests()
    {
        _openRouteClientMock = new Mock<OpenRouteClient>();
        _openRouteValidatorMock = new Mock<OpenRouteValidator>();
        _loggerMock = new Mock<ILogger<DefaultOpenRouteService>>();
        _typeToProfileMap = new Dictionary<TransportType, string>
        {
            { TransportType.Foot, "foot-walking" },
            { TransportType.Car, "driving-car" }
        };
        _service = new DefaultOpenRouteService(_typeToProfileMap, _openRouteClientMock.Object, _openRouteValidatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task GetRouteInformationAsync_ValidInputs_ReturnsRouteInformation()
    {
        // Arrange
        var startCoordinates = new Coordinates(48.2082M, 16.3738M);
        var endCoordinates = new Coordinates(48.207049M, 16.360526M);
        var routeInformation = new RouteInformation(934.0M, 275.4M, startCoordinates, endCoordinates, "encodedPolyline");

        _openRouteClientMock.Setup(client => client.ReverseGeocodeAsync("Vienna"))
            .ReturnsAsync(new ApiResponse<Coordinates>(true, HttpStatusCode.OK, startCoordinates));
        _openRouteClientMock.Setup(client => client.ReverseGeocodeAsync("End"))
            .ReturnsAsync(new ApiResponse<Coordinates>(true, HttpStatusCode.OK, endCoordinates));
        _openRouteClientMock.Setup(client => client.CalculateRouteAsync(startCoordinates, endCoordinates, "driving-car"))
            .ReturnsAsync(new ApiResponse<RouteInformation>(true, HttpStatusCode.OK, routeInformation));

        _openRouteValidatorMock.Setup(validator => validator.ValidateReverseGeocoding("Vienna", It.IsAny<ApiResponse<Coordinates>>()))
            .Returns(ValidationResult.Valid());
        _openRouteValidatorMock.Setup(validator => validator.ValidateReverseGeocoding("End", It.IsAny<ApiResponse<Coordinates>>()))
            .Returns(ValidationResult.Valid());
        _openRouteValidatorMock.Setup(validator => validator.ValidateRouteInformation("Vienna", "End", It.IsAny<ApiResponse<RouteInformation>>()))
            .Returns(ValidationResult.Valid());

        // Act
        var result = await _service.GetRouteInformationAsync("Vienna", "End", TransportType.Car);

        // Assert
        Assert.True(result.IsOk);
        Assert.NotNull(result.RouteInformation);
        Assert.Equal(934.0M, result.RouteInformation.DistanceMeters);
        Assert.Equal(275.4M, result.RouteInformation.EstimatedTimeSeconds);
        Assert.Equal("encodedPolyline", result.RouteInformation.RouteGeometry);
    }

    [Fact]
    public async Task GetRouteInformationAsync_InvalidStartLocation_ReturnsInvalidResult()
    {
        // Arrange
        _openRouteClientMock.Setup(client => client.ReverseGeocodeAsync("InvalidStart"))
            .ReturnsAsync(new ApiResponse<Coordinates>(false, HttpStatusCode.NotFound));

        _openRouteValidatorMock.Setup(validator => validator.ValidateReverseGeocoding("InvalidStart", It.IsAny<ApiResponse<Coordinates>>()))
            .Returns(ValidationResult.WithError("Invalid start location"));

        // Act
        var result = await _service.GetRouteInformationAsync("InvalidStart", "End", TransportType.Car);

        // Assert
        Assert.False(result.IsOk);
        Assert.Equal("Invalid start location", result.ErrorMessage);
    }

    [Fact]
    public async Task GetRouteInformationAsync_InvalidEndLocation_ReturnsInvalidResult()
    {
        // Arrange
        var startCoordinates = new Coordinates(48.2082M, 16.3738M);

        _openRouteClientMock.Setup(client => client.ReverseGeocodeAsync("Vienna"))
            .ReturnsAsync(new ApiResponse<Coordinates>(true, HttpStatusCode.OK, startCoordinates));
        _openRouteClientMock.Setup(client => client.ReverseGeocodeAsync("InvalidEnd"))
            .ReturnsAsync(new ApiResponse<Coordinates>(false, HttpStatusCode.NotFound));

        _openRouteValidatorMock.Setup(validator => validator.ValidateReverseGeocoding("Vienna", It.IsAny<ApiResponse<Coordinates>>()))
            .Returns(ValidationResult.Valid());
        _openRouteValidatorMock.Setup(validator => validator.ValidateReverseGeocoding("InvalidEnd", It.IsAny<ApiResponse<Coordinates>>()))
            .Returns(ValidationResult.WithError("Invalid end location"));

        // Act
        var result = await _service.GetRouteInformationAsync("Vienna", "InvalidEnd", TransportType.Car);

        // Assert
        Assert.False(result.IsOk);
        Assert.Equal("Invalid end location", result.ErrorMessage);
    }

    [Fact]
    public async Task GetRouteInformationAsync_InvalidTransportType_ReturnsInvalidResult()
    {
        // Arrange
        var startCoordinates = new Coordinates(48.2082M, 16.3738M);
        var endCoordinates = new Coordinates(48.207049M, 16.360526M);

        _openRouteClientMock.Setup(client => client.ReverseGeocodeAsync("Start"))
                            .ReturnsAsync(new ApiResponse<Coordinates>(true, HttpStatusCode.OK, startCoordinates));
        _openRouteClientMock.Setup(client => client.ReverseGeocodeAsync("End"))
                            .ReturnsAsync(new ApiResponse<Coordinates>(true, HttpStatusCode.OK, endCoordinates));

        _openRouteValidatorMock.Setup(validator => validator.ValidateReverseGeocoding("Start", It.IsAny<ApiResponse<Coordinates>>()))
                               .Returns(ValidationResult.Valid());
        _openRouteValidatorMock.Setup(validator => validator.ValidateReverseGeocoding("End", It.IsAny<ApiResponse<Coordinates>>()))
                               .Returns(ValidationResult.Valid());

        // Act
        var result = await _service.GetRouteInformationAsync("Start", "End", (TransportType)999);

        // Assert
        Assert.False(result.IsOk);
        Assert.Equal("Invalid transport type", result.ErrorMessage);

        // Verify that the reverse geocoding methods were called
        _openRouteClientMock.Verify(client => client.ReverseGeocodeAsync("Start"), Times.Once);
        _openRouteClientMock.Verify(client => client.ReverseGeocodeAsync("End"), Times.Once);

        // Verify that no route calculation was attempted since the transport type is invalid
        _openRouteClientMock.Verify(client => client.CalculateRouteAsync(It.IsAny<Coordinates>(), It.IsAny<Coordinates>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetRouteInformationAsync_RoutingError_ReturnsInvalidResult()
    {
        // Arrange
        var startCoordinates = new Coordinates(48.2082M, 16.3738M);
        var endCoordinates = new Coordinates(48.207049M, 16.360526M);

        _openRouteClientMock.Setup(client => client.ReverseGeocodeAsync("Vienna"))
            .ReturnsAsync(new ApiResponse<Coordinates>(true, HttpStatusCode.OK, startCoordinates));
        _openRouteClientMock.Setup(client => client.ReverseGeocodeAsync("End"))
            .ReturnsAsync(new ApiResponse<Coordinates>(true, HttpStatusCode.OK, endCoordinates));
        _openRouteClientMock.Setup(client => client.CalculateRouteAsync(startCoordinates, endCoordinates, "driving-car"))
            .ReturnsAsync(new ApiResponse<RouteInformation>(false, HttpStatusCode.NotFound));

        _openRouteValidatorMock.Setup(validator => validator.ValidateReverseGeocoding("Vienna", It.IsAny<ApiResponse<Coordinates>>()))
            .Returns(ValidationResult.Valid());
        _openRouteValidatorMock.Setup(validator => validator.ValidateReverseGeocoding("End", It.IsAny<ApiResponse<Coordinates>>()))
            .Returns(ValidationResult.Valid());
        _openRouteValidatorMock.Setup(validator => validator.ValidateRouteInformation("Vienna", "End", It.IsAny<ApiResponse<RouteInformation>>()))
            .Returns(ValidationResult.WithError("Routing error"));

        // Act
        var result = await _service.GetRouteInformationAsync("Vienna", "End", TransportType.Car);

        // Assert
        Assert.False(result.IsOk);
        Assert.Equal("Routing error", result.ErrorMessage);
    }
}
