using Bookify.Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure;
internal sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    private readonly IPublisher _publisher;

    public ApplicationDbContext(DbContextOptions options, IPublisher publisher) : base(options)
    {
        _publisher = publisher;
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        // when our model is getting configured it will scan this assembly. find our entity configurations and apply them to ef data model.

        base.OnModelCreating(modelBuilder);
    }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var result = await base.SaveChangesAsync(cancellationToken);

        await PublishDomainEventsAsync(); // How to publish DomainEvent using the UnitOfWork pattern. Class implementing UnitOfWork pattern is ApplicationDbContext

        return result;
    }

    private async Task PublishDomainEventsAsync()
    {
        var domainEvents = ChangeTracker.Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.GetDomainEvents();

                entity.ClearDomainEvents(); 

                return domainEvents;
            }).ToList();

        foreach (var domainEvent in domainEvents) { 
            await _publisher.Publish(domainEvent); // this will trigger the respective domain event handler which we defined in the application layer.
        }
    }


}




//entity.ClearDomainEvents(); // This part is important. Because when we publish the domain events. We don't know what could be happening in the handlers
//                // there could be another dbcontext created. which could use the same entity and add another domain event and it might cause strage behaviours.
//                // It prevents duplicate publishing, maintain data consistency, Transactional integritity



// ######### There are some caviates(waringing) to this approach.

//    var result = await base.SaveChangesAsync(cancellationToken);
//    await PublishDomainEventsAsync(); 


// What we are doing here is potentially problematic.
// we are saving changes to the database. which is atomic. This is either going to succeed or fail.

// But then we are pulishing the set of domain events. Which is another transaction all together.
// The domain event handlers could be doing all sorts of things like calling the database or other external services and
// The handlers itself could also fail. This is going to cause an exception which is going to fail the saveChangesAsync method. However orignal call to the 
// database was already completed

// later we will learn a better way of publishing domain events using the outbox pattern.