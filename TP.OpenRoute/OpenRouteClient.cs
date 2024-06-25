using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using TP.Service.Tour;
using TP.Utils;

namespace TP.OpenRoute;

public interface OpenRouteClient
{
    ValueTask<ApiResponse<Coordinates>> ReverseGeocodeAsync(string description);

    ValueTask<ApiResponse<RouteInformation>> CalculateRouteAsync(Coordinates startCoordinates, Coordinates endCoordinates, string profile);
}

public class HttpOpenRouteClient(HttpClient httpClient, ILogger<HttpOpenRouteClient> logger) : OpenRouteClient
{
    public async ValueTask<ApiResponse<Coordinates>> ReverseGeocodeAsync(string description)
    {
        try
        {
            logger.LogInformation("Starting reverse geocode for: {Description}", description);
            HttpResponseMessage response = await httpClient.GetAsync($"geocode/search?text={Uri.EscapeDataString(description)}");

            if (!response.IsSuccessStatusCode)
            {
                ErrorResponse errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>() ?? throw new ArgumentException();
                logger.LogWarning("Failed to reverse geocode {Description} : {StatusCode} - {ErrorCode} - {Message}", description, response.StatusCode, errorResponse.Error.Code, errorResponse.Error.Message);
                return new ApiResponse<Coordinates>(false, response.StatusCode);
            }

            ReverseGeocodeResponse? geocodeResponse = await response.Content.ReadFromJsonAsync<ReverseGeocodeResponse>();

            if (geocodeResponse is null || geocodeResponse.Features.Count == 0)
            {
                logger.LogWarning("No geocode data returned for {Description}", description);
                return new ApiResponse<Coordinates>(false, response.StatusCode);
            }

            GeoFeature firstFeature = geocodeResponse.Features.First();
            Coordinates coordinates = new (Latitude: firstFeature.Geometry.Coordinates[1], Longitude: firstFeature.Geometry.Coordinates[0]);

            logger.LogInformation("Geocode success for {Description}: {Latitude}, {Longitude}", description, coordinates.Latitude, coordinates.Longitude);

            return new ApiResponse<Coordinates>(true, response.StatusCode, coordinates);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing geocode request for {Description}", description);
            return new ApiResponse<Coordinates>(false);
        }
    }

    public async ValueTask<ApiResponse<RouteInformation>> CalculateRouteAsync(Coordinates startCoordinates, Coordinates endCoordinates, string profile)
    {
        try
        {
            string requestUri = $"v2/directions/{profile}?start={startCoordinates.Longitude},{startCoordinates.Latitude}&end={endCoordinates.Longitude},{endCoordinates.Latitude}";
            logger.LogInformation("Sending routing request for profile {Profile}: {RequestUri}", profile, requestUri);

            HttpResponseMessage response = await httpClient.GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
            {
                ErrorResponse errorResponse = await response.Content.ReadFromJsonAsync<ErrorResponse>() ?? throw new ArgumentException();
                logger.LogWarning("Failed to retrieve route between {Start} - {End} with profile {Profile} : {StatusCode} - {ErrorCode} - {Message}", startCoordinates, endCoordinates, profile, response.StatusCode, errorResponse.Error.Code, errorResponse.Error.Message);
                return new ApiResponse<RouteInformation>(false, response.StatusCode, ErrorResponse: errorResponse);
            }

            DirectionsResponse? directionsResponse = await response.Content.ReadFromJsonAsync<DirectionsResponse>();
            if (directionsResponse == null || directionsResponse.Features.Count == 0)
            {
                logger.LogWarning("No routing data returned for start {StartLatLon} to end {EndLatLon}", startCoordinates, endCoordinates);
                return new ApiResponse<RouteInformation>(false, response.StatusCode);
            }

            Feature firstFeature = directionsResponse.Features.First();
            decimal distanceM = firstFeature.Properties.Segments.First().Distance;
            decimal durationS = firstFeature.Properties.Segments.First().Duration;

            logger.LogInformation("Routing success for start {StartLatLon} to end {EndLatLon}: Distance {Distance} meters, Duration {Duration} seconds", startCoordinates, endCoordinates, distanceM, durationS);

            return new ApiResponse<RouteInformation>(true, response.StatusCode, new (distanceM, durationS, startCoordinates,endCoordinates, PolylineEncoder.Encode(firstFeature.Geometry.Coordinates)));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while processing routing request from {StartLatLon} to {EndLatLon}", startCoordinates, endCoordinates);
            return new ApiResponse<RouteInformation>(false);
        }
    }
}