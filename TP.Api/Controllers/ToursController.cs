using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using TP.Api.OpenRoute;
using TP.Api.Utils;
using TP.DataAccess;
using TP.DataAccess.Repositories;
using TP.Domain;
using TP.Service.Tour;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace TP.Api.Controllers;

[Route(ApplicationConstants.OdataDefaultPrefix)]
public class ToursController(
    TourQueryRepository tourQueryRepository,
    TourChangeRepository tourChangeRepository,
    TourService tourService,
    ILogger<ToursController> logger,
    IValidator<TourDTO> tourDtoValidator,
    OpenRouteService openRouteService) : ODataController
{
    [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
    [HttpGet("tours/{id}")]
    [HttpGet("tours({id})")]
    [ProducesResponseType(typeof(Tour), Status200OK)]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public SingleResult<Tour> Get([FromODataUri] Guid id) => SingleResult.Create(tourQueryRepository.GetTourByIdQueryable(id));

    [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
    [HttpGet("tours")]
    [ProducesResponseType(typeof(Tour), Status200OK)]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public IQueryable<Tour> Get() => tourQueryRepository.GetAllToursQueryable();

    [HttpPost("tours")]
    public async Task<IActionResult> Post([FromBody] TourDTO tourDto)
    {
        try
        {
            ValidationResult validationResult = await tourDtoValidator.ValidateAsync(tourDto);

            if (!validationResult.IsValid) return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

            RouteInformationResult routeInformationResult = await openRouteService.GetRouteInformationAsync(tourDto.Start, tourDto.End, tourDto.TransportType);

            if (!routeInformationResult.IsOk) return BadRequest(routeInformationResult.ErrorMessage);

            Tour newTour = tourService.CreateTour(tourDto, routeInformationResult.RouteInformation!);

            await tourChangeRepository.CreateTourAsync(newTour);

            return Created(newTour);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception occured while creating a new tour");
            throw;
        }
    }
}