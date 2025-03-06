using Bookify.Application.Abstractions.Caching;
using Bookify.Application.Abstractions.Messaging;

namespace Bookify.Application.Bookings.GetBooking;
//public sealed record class GetBookingQuery(Guid BookingId) : IQuery<BookingResponse>;
public sealed record class GetBookingQuery(Guid BookingId) : ICachedQuery<BookingResponse>
{
    public string CacheKey => $"bookings-{BookingId}"; // bookingId will help us generate the dynamic key value.

    public TimeSpan? Expiration => null; // this will then use the default expiration time.
}
