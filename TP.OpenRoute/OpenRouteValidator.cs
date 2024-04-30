using System.Net;
using TP.Service.Tour;
using TP.Utils;

namespace TP.OpenRoute;

public interface OpenRouteValidator
{
    ValidationResult ValidateReverseGeocoding(string description, ApiResponse<Coordinates> reverseGeocodingResponse);

    ValidationResult ValidateRouteInformation(string start, string end, ApiResponse<RouteInformation> routeInformationResponse);
}

public class DefaultOpenRouteValidator : OpenRouteValidator
{
    public ValidationResult ValidateReverseGeocoding(string description, ApiResponse<Coordinates> reverseGeocodingResponse)
    {
        if (reverseGeocodingResponse.IsSuccess) return ValidationResult.Valid();

        if (reverseGeocodingResponse.ResponseCode is HttpStatusCode.OK or HttpStatusCode.NotFound)
            return ValidationResult.WithError($"Place with this description {description} could not be reverse geocoded. Try make it more precise and check for errors");

        return ValidationResult.WithError("Something went wrong while processing the request, try again later");
    }

    public ValidationResult ValidateRouteInformation(string start, string end, ApiResponse<RouteInformation> routeInformationResponse)
    {
        if (routeInformationResponse.IsSuccess) return ValidationResult.Valid();

        if (routeInformationResponse.ResponseCode is HttpStatusCode.OK or HttpStatusCode.NotFound)
            return ValidationResult.WithError($"No route found between {start} and {end}. Try something else!");

        if (routeInformationResponse.ErrorResponse?.Error.Code == OpenRouteErrorCodes.TooBigDistance2004)
            return ValidationResult.WithError("Too big distance between points! Must be less than 6000km.");

        return ValidationResult.WithError("Route in this tour could not be computed, try again");
    }
}