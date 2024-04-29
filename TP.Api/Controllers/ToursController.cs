using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using TP.Api.Http;
using TP.Api.Utils;
using TP.DataAccess;
using TP.Domain;
using TP.Service.Tour;
using TP.Utils;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace TP.Api.Controllers;

[Route(ApplicationConstants.OdataDefaultPrefix)]
public class ToursController(
    TourRepository tourRepository,
    ILogger<ToursController> logger,
    OpenRouteValidator openRouteValidator,
    IMapper mapper,
    OpenRouteClient openRouteClient) : ODataController
{
    private static readonly Dictionary<TransportType, string> TypeToProfileMap = new()
    {
        { TransportType.Foot, "foot-walking" },
        { TransportType.Car, "driving-car" },
        { TransportType.Truck, "driving-hgv" },
        { TransportType.Bicycle, "cycling-regular" },
        { TransportType.Foot, "cycling-electric" }
    };

    [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
    [HttpGet("tours/{id}")]
    [HttpGet("tours({id})")]
    [ProducesResponseType(typeof(Tour), Status200OK)]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public SingleResult<Tour> Get([FromODataUri] Guid id) => SingleResult.Create(tourRepository.GetTourByIdQueryable(id));

    [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
    [HttpGet("tours")]
    [ProducesResponseType(typeof(Tour), Status200OK)]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public IQueryable<Tour> Get() => tourRepository.GetAllToursQueryable();

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] TourDTO tourDto)
    {
        try
        {
            ApiResponse<GeoPoint> startReverseGeocodingResponse = await openRouteClient.ReverseGeocodeAsync(tourDto.Start);

            ValidationResult startValidationResult = openRouteValidator.ValidateReverseGeocoding(tourDto.Start, startReverseGeocodingResponse);

            if (!startValidationResult.IsValid) return BadRequest(startValidationResult.ErrorMessage);

            ApiResponse<GeoPoint> endReverseGeocodingResponse = await openRouteClient.ReverseGeocodeAsync(tourDto.End);

            ValidationResult endValidationResult = openRouteValidator.ValidateReverseGeocoding(tourDto.End, endReverseGeocodingResponse);

            if (!endValidationResult.IsValid) return BadRequest(endValidationResult.ErrorMessage);

            if (!TypeToProfileMap.TryGetValue(tourDto.TransportType, out string? profile)) return BadRequest("Invalid transport type");

            var routeInformationResponse = await openRouteClient.CalculateRouteAsync(startReverseGeocodingResponse.Response!, endReverseGeocodingResponse.Response!, profile);

            ValidationResult routeInfoValidationResult = openRouteValidator.ValidateRouteInformation(tourDto.Start, tourDto.End, routeInformationResponse);

            if (!routeInfoValidationResult.IsValid) return BadRequest(routeInfoValidationResult.ErrorMessage);

            Tour newTour = mapper.Map<Tour>(tourDto);
            newTour.Distance = routeInformationResponse.Response!.DistanceM;
            newTour.EstimatedTime = TimeSpan.FromSeconds((double)routeInformationResponse.Response.EstimatedDurationS);

            return Created(newTour);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception occured while creating a new tour");
            throw;
        }
    }
}