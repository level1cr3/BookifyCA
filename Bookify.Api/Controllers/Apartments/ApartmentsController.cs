using Bookify.Application.Apartments.SearchApartments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Controllers.Apartments;

[Authorize]
[ApiController] // It is there to tell the dotnet runtime that this is suppose to be a api controller. It more light weight and requires less services and little bit more performant then MVC style controller.
[Route("api/apartments")]//instead of controller template we are going to hardcode the route. just to make things explicit.
public class ApartmentsController : ControllerBase
{
    private readonly ISender _sender;

    public ApartmentsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> SearchApartments(DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
    {
        var query = new SearchApartmentsQuery(startDate, endDate);

        var result = await _sender.Send(query, cancellationToken);

        return Ok(result.Value); // because this query can never fail. we will directly return result.value.
    }
}

// this controller inherits from controllerBase. This is also what an api controller should do. Because we don't need everything that is exposed by the base controller.