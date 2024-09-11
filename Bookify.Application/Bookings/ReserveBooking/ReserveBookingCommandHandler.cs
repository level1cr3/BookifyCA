using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Application.Exceptions;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Apartments;
using Bookify.Domain.Bookings;
using Bookify.Domain.Users;

namespace Bookify.Application.Bookings.ReserveBooking;
internal sealed class ReserveBookingCommandHandler : ICommandHandler<ReserveBookingCommand, Guid>
{
    private readonly IUserRepository _userRepository;
    private readonly IApartmentRepository _apartmentRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly PricingService _pricingService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public ReserveBookingCommandHandler(
        IUserRepository userRepository,
        IApartmentRepository apartmentRepository,
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork,
        PricingService pricingService,
        IDateTimeProvider dateTimeProvider)
    {
        _userRepository = userRepository;
        _apartmentRepository = apartmentRepository;
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
        _pricingService = pricingService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<Guid>> Handle(ReserveBookingCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<Guid>(UserErrors.NotFound);
        }


        var apartment = await _apartmentRepository.GetByIdAsync(request.ApartmentId, cancellationToken);

        if (apartment is null)
        {
            return Result.Failure<Guid>(ApartmentErrors.NotFound);
        }

        // we will introduce the validation later so wrong date never reaches the handler.
        var duration = DateRange.Create(request.StartDate, request.EndDate);


        if (await _bookingRepository.IsOverlappingAsync(apartment, duration, cancellationToken))
        {
            return Result.Failure<Guid>(BookingErrors.Overlap);
        }

        try
        {

            var booking = Booking.Reserve(
                apartment,
                user.Id,
                duration,
                _dateTimeProvider.UtcNow,
                _pricingService);

            _bookingRepository.Add(booking);
            var isSaved = await _unitOfWork.SaveChangesAsync(cancellationToken) > 0;
            return isSaved ? booking.Id : Result.Failure<Guid>(BookingErrors.NotReserved);
        }
        catch (ConcurrencyException)
        {
            return Result.Failure<Guid>(BookingErrors.Overlap);
        }
    }
}


// promts single responsiblity because this class handles only one command




// ## await _unitOfWork.SaveChangesAsync(cancellationToken)
// Did we just introduced a 'race condition'?
// what is a race condition ?
// A race condition is a situation that occurs in concurrent or multi-threaded systems when the behavior of a program depends on the timing or order of execution of its threads or processes. It happens when two or more threads or processes access shared resources (such as variables, memory, files, etc.) and try to change them simultaneously without proper synchronization, leading to unpredictable results.

// Solution:
// Race conditions can be avoided by properly synchronizing access to shared resources.Common techniques include:

// Locks/Mutexes: Ensure that only one thread can access a resource at a time.
// Atomic Operations: Use atomic operations that are guaranteed to complete without interruption.
// Thread-Safe Data Structures: Use data structures that are designed for safe access by multiple threads.

// We will solve the race condition by using optimistic concurrency.
// Current problem is we could have 2 seprate transcations both trying to presist the booking
// There are 2 ways to solve this.
// 1. pessimistic Locking. 
// It means creating a transaction with some of the more constructive isolation level
// 2 optimistic locking
// It means you have a concurrency token present on your entities. The reason for using this is because it is for promance and it doesn't require locking a certain
// number of rows in the database for extended period of time
