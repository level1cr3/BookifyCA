using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Abstractions.Data;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Bookings;
using Dapper;

namespace Bookify.Application.Bookings.GetBooking;
internal sealed class GetBookingQueryHandler : IQueryHandler<GetBookingQuery, BookingResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IUserContext _userContext;

    public GetBookingQueryHandler(ISqlConnectionFactory sqlConnectionFactory, IUserContext userContext)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _userContext = userContext;
    }

    public async Task<Result<BookingResponse>> Handle(GetBookingQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnectionFactory.CreateConnection();

        // executing query with dapper is simple we just call one of the extensions methods that are exposed on db connection interface.

        const string sql = """
            SELECT 
                id AS Id,
                apartment_id AS ApartmentId,
                user_id AS UserId,
                status AS Status,
                price_for_period_amount AS PriceAmount,
                price_for_period_currency AS PriceCurrency,
                cleaning_fee_amount AS CleaningFeeAmount,
                cleaning_fee_currency AS CleaningFeeCurrency,
                amenities_up_charge_amount AS AmenitiesUpChargeAmount,
                amenities_up_charge_currency AS AmenitiesUpChargeCurrency,
                total_price_amount AS TotalPriceAmount,
                total_price_currency AS TotalPriceCurrency,
                duration_start AS DurationStart,
                duration_end AS DurationEnd,
                created_on_utc AS CreatedOnUtc
            FROM bookings
            WHERE id = @BookingId
            """;


        var booking = await connection.QueryFirstOrDefaultAsync<BookingResponse>(sql, new { request.BookingId });

        if (booking is null || booking.UserId != _userContext.UserId )
        {
            return Result.Failure<BookingResponse>(BookingErrors.NotFound);
        }


        return booking;
    }
}


// This is a pragmatic clean architecture.
// we are using sql to directly access our read model and directly return response from our query. without any indirection. Downside is that we are not completely
// abstracting our persistence concerns in this case our sql database from our query handler But the benifits are too many to count.
// 1. it is simple and performant because of dapper
// 2. Being able to define your readmodel in database level by defining database view. (so you don't have write complecated query in code)

// But what about not being able to switch a database?
// If you are switching db then you have a lot of problem. then just using sql in application layer.
// If you end up moving from sql to document database or column store. then you gonna have to write your entire persistence layer.