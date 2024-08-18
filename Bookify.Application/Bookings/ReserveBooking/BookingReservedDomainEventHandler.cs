using Bookify.Application.Abstractions.Email;
using Bookify.Domain.Bookings;
using Bookify.Domain.Bookings.Events;
using Bookify.Domain.Users;
using MediatR;

namespace Bookify.Application.Bookings.ReserveBooking;
internal sealed class BookingReservedDomainEventHandler : INotificationHandler<BookingReservedDomainEvent>
{
    private readonly IBookingRepository _bookingRepository;

    private readonly IUserRepository _userRepository;

    private readonly IEmailService _emailService;

    public BookingReservedDomainEventHandler(
        IBookingRepository bookingRepository,
        IUserRepository userRepository,
        IEmailService emailService)
    {
        _bookingRepository = bookingRepository;
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task Handle(BookingReservedDomainEvent notification, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdAsync(notification.BookingId, cancellationToken);

        if (booking == null)
        {
            return;
        }

        var user = await _userRepository.GetByIdAsync(booking.UserId, cancellationToken);

        if (user == null)
        {
            return;
        }

        await _emailService.SendAsync(
            user.Email,
            "Booking Reserved!",
            "You have 10 minutes to confirm this booking.");
    }
}


// naming convention we are using is : we specify what is the command, query or event and then we append handler.

// the depency injection configuation that we added for mediatr is going to take care of wiring up this class.

// we are corrently using the repositories and this is the most performant approach. But this is something we can live with in our command handlers.
// But we have to ask ourself if this is the approach you want to take in the event handler. But we could also take the approach of executing sql queries directly iin
// database using dapper.