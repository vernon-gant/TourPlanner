using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using TP.Utils;

namespace TP.OpenRoute.Tests;

public class HttpOpenRouteClientTests
{
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly Mock<ILogger<HttpOpenRouteClient>> _loggerMock;
    private readonly HttpClient _httpClient;
    private readonly HttpOpenRouteClient _client;

    public HttpOpenRouteClientTests()
    {
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
        _loggerMock = new Mock<ILogger<HttpOpenRouteClient>>();
        _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
        {
            BaseAddress = new Uri("https://api.openrouteservice.org")
        };
        _client = new HttpOpenRouteClient(_httpClient, _loggerMock.Object);
    }

    [Fact]
    public async Task ReverseGeocodeAsync_Success_ReturnsCoordinates()
    {
        // Arrange
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new ReverseGeocodeResponse
            {
                Features = new List<GeoFeature>
                {
                    new()
                    {
                        Geometry = new GeoGeometry
                        {
                            Coordinates = new List<decimal> { 16.3738M, 48.2082M }
                        }
                    }
                }
            })
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _client.ReverseGeocodeAsync("Vienna");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(48.2082M, result.Response!.Latitude);
        Assert.Equal(16.3738M, result.Response.Longitude);
    }

    [Fact]
    public async Task ReverseGeocodeAsync_NotFound_ReturnsError()
    {
        // Arrange
        var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = JsonContent.Create(new ErrorResponse
            {
                Error = new Error
                {
                    Code = 404,
                    Message = "Not Found"
                }
            })
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _client.ReverseGeocodeAsync("Unknown");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(HttpStatusCode.NotFound, result.ResponseCode);
    }

    [Fact]
    public async Task CalculateRouteAsync_Success_ReturnsRouteInformation()
    {
        // Arrange
        var responseMessage = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new DirectionsResponse
            {
                Features = new List<Feature>
                {
                    new Feature
                    {
                        Properties = new Properties
                        {
                            Segments =
                            [
                                new()
                                {
                                    Distance = 934.0M,
                                    Duration = 275.4M
                                }
                            ]
                        },
                        Geometry = new Geometry
                        {
                            Coordinates = new List<List<decimal>>
                            {
                                new() { 16.358517M, 48.211397M }
                            }
                        }
                    }
                }
            })
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _client.CalculateRouteAsync(new Coordinates(48.211397M, 16.358517M), new Coordinates(48.207049M, 16.360526M), "cycling-regular");

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(934.0M, result.Response!.DistanceMeters);
        Assert.Equal(275.4M, result.Response.EstimatedTimeSeconds);
    }

    [Fact]
    public async Task CalculateRouteAsync_NotFound_ReturnsError()
    {
        // Arrange
        var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = JsonContent.Create(new ErrorResponse
            {
                Error = new Error
                {
                    Code = 404,
                    Message = "Not Found"
                }
            })
        };

        _httpMessageHandlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _client.CalculateRouteAsync(new Coordinates(48.211397M, 16.358517M), new Coordinates(48.207049M, 16.360526M), "cycling-regular");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(HttpStatusCode.NotFound, result.ResponseCode);
    }
}