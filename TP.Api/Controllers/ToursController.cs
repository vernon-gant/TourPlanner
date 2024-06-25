using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.IdentityModel.Tokens;
using TP.Api.Utils;
using TP.DataAccess.Repositories;
using TP.Domain;
using TP.Export;
using TP.Import;
using TP.OpenRoute;
using TP.Service.Tour;
using TP.Utils;
using static Microsoft.AspNetCore.Http.StatusCodes;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace TP.Api.Controllers;

[Route(ApplicationConstants.OdataDefaultPrefix)]
public class ToursController(
    TourQueryRepository tourQueryRepository,
    TourChangeRepository tourChangeRepository,
    IEnumerable<TourExporter> tourExporters,
    IEnumerable<TourImporter> tourImporters,
    TourService tourService,
    ILogger<ToursController> logger,
    IValidator<TourDTO> tourDtoValidator,
    IValidator<TourUpdateDTO> tourUpdateDtoValidator,
    FullTextSearchRepository fullTextSearchRepository,
    OpenRouteService openRouteService) : ODataController
{
    [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
    [HttpGet("tours/{id}")]
    [HttpGet("tours({id})")]
    [ProducesResponseType(typeof(Tour), Status200OK)]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public SingleResult<Tour> Get([FromODataUri] Guid id) =>
        SingleResult.Create(tourQueryRepository.GetTourByIdQueryable(id));

    [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
    [HttpGet("tours")]
    [ProducesResponseType(typeof(Tour), Status200OK)]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public IQueryable<Tour> Get() => tourQueryRepository.GetAllToursQueryable();

    [HttpPost("tours")]
    [ProducesResponseType(typeof(Tour), Status201Created)]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
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

    [HttpPut("tours/{tourId}")]
    [ProducesResponseType(Status204NoContent)]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> Put(Guid tourId, [FromBody] TourUpdateDTO tourUpdateDto)
    {
        ValidationResult validationResult = await tourUpdateDtoValidator.ValidateAsync(tourUpdateDto);

        if (!validationResult.IsValid) return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

        Tour? existingTour = await tourQueryRepository.GetTourByIdAsync(tourId);

        if (existingTour == null) return NotFound();

        existingTour.Name = tourUpdateDto.Name;
        existingTour.Description = tourUpdateDto.Description;

        await tourChangeRepository.UpdateTourAsync(existingTour);

        return NoContent();
    }

    [HttpDelete("tours/{tourId}")]
    [ProducesResponseType(Status204NoContent)]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> Delete(Guid tourId)
    {
        Tour? existingTour = await tourQueryRepository.GetTourByIdAsync(tourId);
        if (existingTour == null) return NotFound();

        await tourChangeRepository.DeleteTourAsync(tourId);

        return NoContent();
    }

    [HttpPost("tours/export")]
    [ProducesResponseType(typeof(FileStreamResult), Status200OK)]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    [Produces(ContentTypes.Xlsx)]
    public async Task<IActionResult> Export([FromBody] List<Guid> tourIds, [FromQuery] bool withTourLogs, string format)
    {
        if (format.IsNullOrEmpty()) return BadRequest("Fill out the format of destination file");

        if (tourIds.IsNullOrEmpty()) return BadRequest("There must be at least one tour");

        HashSet<Guid> uniqueTourIds = tourIds.ToHashSet();

        if (uniqueTourIds.Count != tourIds.Count) return BadRequest("Duplicate tour ids, recheck them");

        List<Tour> foundTours = await tourQueryRepository.GetByIds(uniqueTourIds, withTourLogs);

        if (foundTours.Count != tourIds.Count) return BadRequest("Some of the tours do not exist, recheck the payload");

        TourExporter? exporter = tourExporters.FirstOrDefault(tourExporter => tourExporter.CanHandle(format));

        if (exporter is null) return BadRequest("File format is not supported");

        OperationResult<ExportResult> exportResult = exporter.ExportTours(foundTours, withTourLogs);

        if (!exportResult.IsOk) return BadRequest("Something went wrong during export, try again");

        return File(exportResult.Result!.FileStream, exportResult.Result.ContentType, "export.xlsx");
    }

    [HttpPost("tours/import")]
    [ProducesResponseType(typeof(List<Tour>), Status200OK)]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public async ValueTask<IActionResult> Import([FromForm] FileForm fileForm, string format)
    {
        if (fileForm.SubmittedFile.Length == 0) return BadRequest("Empty file");

        if (format.IsNullOrEmpty()) return BadRequest("Fill out the format of destination file");

        TourImporter? importer = tourImporters.FirstOrDefault(tourImporter => tourImporter.CanHandle(format));

        if (importer is null) return BadRequest("File format is not supported");

        OperationResult<List<Tour>> importResult = importer.Import(fileForm.SubmittedFile.OpenReadStream());

        if (!importResult.IsOk) return BadRequest("Somethings went wrong during import, try again");

        await tourChangeRepository.CreateRangeAsync(importResult.Result!);

        return Ok();
    }

    [HttpGet("tours/search")]
    [ProducesResponseType(typeof(Tour), Status200OK)]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> SearchTours([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return BadRequest("Search query cannot be empty.");

        var searchStatus = await fullTextSearchRepository.SearchToursAsync(query);

        return searchStatus switch
        {
            DataAccess.Repositories.Ok => Ok(new { value = fullTextSearchRepository.FoundTours.Select(tour => new { tour.Id, tour.Name, tour.CreatedOn }) }),
            DatabaseError => StatusCode(500, "A database error occurred."),
            _ => StatusCode(500, "An unknown error occurred."),
        };
    }
}