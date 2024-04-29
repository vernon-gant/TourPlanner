using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using TP.Api.Utils;
using TP.DataAccess;
using TP.DataAccess.Repositories;
using TP.Database;
using TP.Domain;
using TP.Service.TourLog;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace TP.Api.Controllers;

[Route(ApplicationConstants.OdataDefaultPrefix)]
public class TourLogsController(
    TourLogQueryRepository tourLogQueryRepository,
    TourQueryRepository tourQueryRepository,
    TourLogChangeRepository tourLogChangeRepository,
    TourChangeRepository tourChangeRepository,
    TourLogService tourLogService,
    IValidator<TourLogDTO> tourLogDtoValidator,
    IMapper mapper,
    ILogger<ToursController> logger) : ODataController
{
    [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
    [HttpGet("tour-logs/{id}")]
    [HttpGet("tour-logs({id})")]
    [ProducesResponseType(typeof(TourLog), Status200OK)]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public SingleResult<TourLog> Get([FromODataUri] Guid id) => SingleResult.Create(tourLogQueryRepository.GetTourLogByIdQueryable(id));

    [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
    [HttpGet("tour-logs")]
    [ProducesResponseType(typeof(TourLog), Status200OK)]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public IQueryable<TourLog> Get() => tourLogQueryRepository.GetAllTourLogsQueryable();

    [HttpPost("tour-logs/{tourId}")]
    [ProducesResponseType(typeof(TourLog), Status201Created)]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> Post(Guid tourId, [FromBody] TourLogDTO tourLogDto)
    {
        try
        {
            ValidationResult validationResult = await tourLogDtoValidator.ValidateAsync(tourLogDto);

            if (!validationResult.IsValid) return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

            Tour? tour = await tourQueryRepository.GetTourByIdAsync(tourId);

            if (tour is null) return NotFound($"Tour with id {tourId} was not found");

            TourLog newTourLog = tourLogService.CreateTourLog(tourLogDto, tour);

            await tourLogChangeRepository.CreateTourLogAsync(newTourLog);
            await tourChangeRepository.UpdateTourAsync(tour);

            return Created(newTourLog);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception occured while creating a new tour log");
            throw;
        }
    }

    [HttpPut("tour-logs/{tourLogId}")]
    [ProducesResponseType(Status204NoContent)]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> Put(Guid tourLogId, [FromBody] TourLogDTO tourLogDto)
    {
        ValidationResult validationResult = await tourLogDtoValidator.ValidateAsync(tourLogDto);

        if (!validationResult.IsValid) return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));

        TourLog? existingTourLog = await tourLogQueryRepository.GetTourLogByIdAsync(tourLogId);

        if (existingTourLog == null) return NotFound();

        await tourLogChangeRepository.UpdateTourLogAsync(mapper.Map(tourLogDto, existingTourLog));

        return NoContent();
    }

    [HttpDelete("tour-logs/{tourLogId}")]
    [ProducesResponseType(Status204NoContent)]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public async Task<IActionResult> Delete(Guid tourLogId)
    {
        TourLog? existingTourLog = await tourLogQueryRepository.GetTourLogByIdAsync(tourLogId);

        if (existingTourLog == null) return NotFound();

        await tourLogChangeRepository.DeleteTourLogAsync(tourLogId);

        return NoContent();
    }

}