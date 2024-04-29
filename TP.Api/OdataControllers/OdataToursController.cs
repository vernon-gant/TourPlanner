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
public class OdataToursController(AppDbContext dbContext) : ODataController
{
    [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
    [HttpGet("tours/{id}")]
    [HttpGet("tours({id})")]
    [ProducesResponseType(typeof(Tour), Status200OK)]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public SingleResult<Tour> Get([FromODataUri] Guid id)
    {
        IQueryable<Tour> foundTour = dbContext.Tours.Where(tour => tour.Id == id);
        return SingleResult.Create(foundTour);
    }

    [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All)]
    [HttpGet("tours")]
    [ProducesResponseType(typeof(Tour), Status200OK)]
    [ProducesDefaultResponseType(typeof(ProblemDetails))]
    public IQueryable<Tour> Get()
    {
        return dbContext.Tours;
    }
}