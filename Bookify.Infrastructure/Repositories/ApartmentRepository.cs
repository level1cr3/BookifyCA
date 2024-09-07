using Bookify.Domain.Apartments;

namespace Bookify.Infrastructure.Repositories;
internal sealed class ApartmentRepository(ApplicationDbContext dbContext) 
    : Repository<Apartment>(dbContext), IApartmentRepository
{
}
