using System.Text.Json.Serialization;
using TP.Service.Tour;
using TP.Utils;

namespace TP.Api.Http;

public interface OpenRouteClient
{
    ValueTask<ApiResponse<GeoPoint>> ReverseGeocodeAsync(string description);

    ValueTask<ApiResponse<RouteInformation>> CalculateRouteAsync(GeoPoint start, GeoPoint end, string profile);
}

public class HttpOpenRouteClient(HttpClient httpClient, ILogger<HttpOpenRouteClient> logger) : OpenRouteClient
{
    public async ValueTask<ApiResponse<GeoPoint>> ReverseGeocodeAsync(string description)
    {
        try
        {
            logger.LogInformation("Starting reverse geocode for: {Description}", description);
            HttpResponseMessage response = await httpClient.GetAsync($"geocode/search?text={Uri.EscapeDataString(description)}");

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Failed to retrieve geocode data for {Description}: {StatusCode}", description, response.StatusCode);
                return new ApiResponse<GeoPoint>(false, response.StatusCode);
            }

            ReverseGeocodeResponse? geocodeResponse = await response.Content.ReadFromJsonAsync<ReverseGeocodeResponse>();

            if (geocodeResponse is null || geocodeResponse.Features.Count == 0)
            {
                logger.LogWarning("No geocode data returned for {Description}", description);
                return new ApiResponse<GeoPoint>(false, response.StatusCode);
            }

            GeoFeature firstFeature = geocodeResponse.Features.First();
            GeoPoint geoPoint = new (Latitude: firstFeature.Geometry.Coordinates[1], Longitude: firstFeature.Geometry.Coordinates[0]);

            logger.LogInformation("Geocode success for {Description}: {Latitude}, {Longitude}", description, geoPoint.Latitude, geoPoint.Longitude);

            return new ApiResponse<GeoPoint>(true, response.StatusCode, geoPoint);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing geocode request for {Description}", description);
            return new ApiResponse<GeoPoint>(false);
        }
    }

    public async ValueTask<ApiResponse<RouteInformation>> CalculateRouteAsync(GeoPoint start, GeoPoint end, string profile)
    {
        try
        {
            string requestUri = $"v2/directions/{profile}?start={start.Longitude},{start.Latitude}&end={end.Longitude},{end.Latitude}";
            logger.LogInformation("Sending routing request for profile {Profile}: {RequestUri}", profile, requestUri);

            HttpResponseMessage response = await httpClient.GetAsync(requestUri);

            if (!response.IsSuccessStatusCode)
            {
                logger.LogWarning("Failed to retrieve route data: {StatusCode}", response.StatusCode);
                return new ApiResponse<RouteInformation>(false, response.StatusCode);
            }

            DirectionsResponse? directionsResponse = await response.Content.ReadFromJsonAsync<DirectionsResponse>();
            if (directionsResponse == null || directionsResponse.Features.Count == 0)
            {
                logger.LogWarning("No routing data returned for start {StartLatLon} to end {EndLatLon}", start, end);
                return new ApiResponse<RouteInformation>(false, response.StatusCode);
            }

            Feature firstFeature = directionsResponse.Features.First();
            decimal distanceM = firstFeature.Properties.Segments.First().Distance;
            decimal durationS = firstFeature.Properties.Segments.First().Duration;

            logger.LogInformation("Routing success for start {StartLatLon} to end {EndLatLon}: Distance {Distance} meters, Duration {Duration} seconds", start, end, distanceM, durationS);

            return new ApiResponse<RouteInformation>(true, response.StatusCode, new (distanceM, durationS));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while processing routing request from {StartLatLon} to {EndLatLon}", start, end);
            return new ApiResponse<RouteInformation>(false);
        }
    }
}

public class ReverseGeocodeResponse
{
    public List<GeoFeature> Features { get; set; } = null!;
}

public class GeoFeature
{
    public GeoGeometry Geometry { get; set; }
}

public class GeoGeometry
{
    public List<decimal> Coordinates { get; set; }
}

public class DirectionsResponse
{
    public List<Feature> Features { get; set; }
}

public class Feature
{
    public Properties Properties { get; set; }
}

public class Properties
{
    public List<Segment> Segments { get; set; }
}

public class Segment
{
    public decimal Distance { get; set; }

    public decimal Duration { get; set; }
}