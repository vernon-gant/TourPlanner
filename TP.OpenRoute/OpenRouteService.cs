using Microsoft.Extensions.Logging;
using TP.Domain;
using TP.Utils;

namespace TP.OpenRoute;

public interface OpenRouteService
{
    ValueTask<RouteInformationResult> GetRouteInformationAsync(string start, string end, TransportType transportType);
}

public class DefaultOpenRouteService(Dictionary<TransportType, string> typeToProfileMap, OpenRouteClient openRouteClient, OpenRouteValidator openRouteValidator, ILogger<DefaultOpenRouteService> logger) : OpenRouteService
{
    public async ValueTask<RouteInformationResult> GetRouteInformationAsync(string start, string end, TransportType transportType)
    {
        try
        {
            ApiResponse<Coordinates> startReverseGeocodingResponse = await openRouteClient.ReverseGeocodeAsync(start);

            ValidationResult startValidationResult = openRouteValidator.ValidateReverseGeocoding(start, startReverseGeocodingResponse);

            if (!startValidationResult.IsValid) return RouteInformationResult.Invalid(startValidationResult.ErrorMessage!);

            ApiResponse<Coordinates> endReverseGeocodingResponse = await openRouteClient.ReverseGeocodeAsync(end);

            ValidationResult endValidationResult = openRouteValidator.ValidateReverseGeocoding(end, endReverseGeocodingResponse);

            if (!endValidationResult.IsValid) return RouteInformationResult.Invalid(endValidationResult.ErrorMessage!);

            if (!typeToProfileMap.TryGetValue(transportType, out string? profile)) return RouteInformationResult.Invalid("Invalid transport type");

            var routeInformationResponse = await openRouteClient.CalculateRouteAsync(startReverseGeocodingResponse.Response!, endReverseGeocodingResponse.Response!, profile);

            ValidationResult routeInfoValidationResult = openRouteValidator.ValidateRouteInformation(start, end, routeInformationResponse);

            return !routeInfoValidationResult.IsValid ? RouteInformationResult.Invalid(routeInfoValidationResult.ErrorMessage!) : RouteInformationResult.Ok(routeInformationResponse.Response!);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while processing the route information");
            throw;
        }
    }
}