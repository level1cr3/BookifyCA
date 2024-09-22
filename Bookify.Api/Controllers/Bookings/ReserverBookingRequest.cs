namespace Bookify.Api;

public sealed record ReserverBookingRequest(Guid ApartmentId,
                                            Guid UserId,
                                            DateOnly StartDate,
                                            DateOnly EndDate);
