using FluentValidation;

namespace Bookify.Application.Bookings.ReserveBooking;
public class ReserveBookingCommandValidator : AbstractValidator<ReserveBookingCommand>
{
    public ReserveBookingCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        
        RuleFor(c => c.ApartmentId).NotEmpty();
        
        RuleFor(c => c.StartDate).LessThan(c => c.EndDate);
    }
}

// these are the validation we could run on ReserveBookingCommand and then they will be triggered in the validaitonBehaviour. when the pipeline is executed

// we also need to register our validator with dependency injection.