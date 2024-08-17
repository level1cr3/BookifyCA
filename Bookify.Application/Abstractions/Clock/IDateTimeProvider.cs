namespace Bookify.Application.Abstractions.Clock;
public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}

// The benifit of this approach is that this is completely testable. If you can't reliably test your code that is working with the date time object. Then you risk
// introducing many potential bugs. and using this approach we provide mock data for testing easily.
