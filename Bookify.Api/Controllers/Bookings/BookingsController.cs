using Asp.Versioning;
using Bookify.Application.Bookings.GetBooking;
using Bookify.Application.Bookings.ReserveBooking;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Api.Controllers.Bookings;

[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/bookings")]
public class BookingsController(ISender sender) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBooking(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetBookingQuery(id);

        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? Ok(result.Value) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> ReserveBooking(ReserverBookingRequest request, CancellationToken cancellationToken)
    {
        var command = new ReserveBookingCommand(request.ApartmentId,
                                                request.UserId,
                                                request.StartDate,
                                                request.EndDate);


        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return CreatedAtAction(nameof(GetBooking), new { id = result.Value }, result.Value);

        //return 201 resoponse and response will have location header with route to getbooking endpoint and the id of newly created booking.
    }


    //Note : The reason for using 'ReserverBookingRequest instead of using ReserveBookingCommand' for api enpoint.
    // ReserveBookingCommand is part of my internal api and it is only valid inside of the scope of my application layer. we don't want to expose it at api endpoint
    // because then we are coupling our endpoint and defination of our command. This is leaking information about our internal api which we never want to do.
    // secondly it is preventing the command from evoling separtly from your endpoint. you might want to introduce some additional info in the command which we
    // can populate inside the api request and you wouldn't nessarly want to expose all of these values as part of api contract.



}
