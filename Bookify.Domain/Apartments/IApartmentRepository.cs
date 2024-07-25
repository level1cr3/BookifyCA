namespace Bookify.Domain.Apartments;
public interface IApartmentRepository
{
    Task<Apartment?> GetByIdAsync(Guid id, CancellationToken cancellation = default);
}

// we will use this later on application layer to execute the required business operation.
