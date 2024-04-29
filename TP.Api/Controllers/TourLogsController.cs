using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Formatter;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using TP.Api.Utils;
using TP.Database;
using TP.Domain;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace TP.Api.OdataControllers;

[Route(ApplicationConstants.OdataDefaultPrefix)]
public class TourLogsController(AppDbContext dbContext) : ODataController
{
    [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
    [HttpGet("tour-logs/{id}")]
    [HttpGet("tour-logs({id})")]
    [ProducesResponseType(typeof(TourLog), Status200OK)]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public SingleResult<TourLog> Get([FromODataUri] Guid id)
    {
        IQueryable<TourLog> foundTourLog = dbContext.TourLogs.Where(tour => tour.Id == id);
        return SingleResult.Create(foundTourLog);
    }

    [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
    [HttpGet("tour-logs")]
    [ProducesResponseType(typeof(TourLog), Status200OK)]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public IQueryable<TourLog> Get()
    {
        return dbContext.TourLogs;
    }
}