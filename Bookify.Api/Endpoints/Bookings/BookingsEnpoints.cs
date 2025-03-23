using Bookify.Application.Bookings.GetBooking;
using Bookify.Application.Bookings.ReserveBooking;
using MediatR;
using System.Runtime.CompilerServices;

namespace Bookify.Api.Endpoints.Bookings;

public static class BookingsEnpoints
{
    public static IEndpointRouteBuilder MapBookingEnpoints(this IEndpointRouteBuilder builder)
    {
        //builder.MapGet("api/v{version:apiVersion}/minimalBookings/{id}", GetBooking).RequireAuthorization().WithName(nameof(GetBooking)).HasApiVersion(1); // to map to specific endpoint version


        var apiVersionset = builder.NewApiVersionSet().HasApiVersion(new Asp.Versioning.ApiVersion(1)).ReportApiVersions().Build();

        builder.MapGet("api/v{version:apiVersion}/minimalBookings/{id}", GetBooking).RequireAuthorization().WithName(nameof(GetBooking)).WithApiVersionSet(apiVersionset);

        builder.MapPost("api/v{version:apiVersion}/minimalBookings", ReserveBooking).RequireAuthorization().WithApiVersionSet(apiVersionset);

        return builder;
    }

    private static async Task<IResult> GetBooking(Guid id, ISender sender, CancellationToken cancellationToken)
    {
        var query = new GetBookingQuery(id);

        var result = await sender.Send(query, cancellationToken);

        return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound(result.Value);
    }


    private static async Task<IResult> ReserveBooking(ReserverBookingRequest request, ISender sender, CancellationToken cancellationToken)
    {
        var command = new ReserveBookingCommand(request.ApartmentId, request.UserId, request.StartDate, request.EndDate);

        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return Results.BadRequest(result.Error);
        }

        return Results.CreatedAtRoute(nameof(GetBooking), new { id = result.Value }, result.Value);
    }

}
