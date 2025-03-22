using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Exceptions;
using Bookify.Domain.Abstractions;
using Bookify.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Data;

namespace Bookify.Infrastructure;
public sealed class ApplicationDbContext : DbContext, IUnitOfWork
{
    private static readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All, // this is because we want to serialize the type of our domainEvents. This will allow us
                                                 // to deserialize it later into an Instance of IDomainEvent.
    };

    private readonly IDateTimeProvider _dateTimeProvider;

    public ApplicationDbContext(DbContextOptions options, IDateTimeProvider dateTimeProvider) : base(options)
    {
        _dateTimeProvider = dateTimeProvider;
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        // when our model is getting configured it will scan this assembly. find our entity configurations and apply them to ef data model.

        base.OnModelCreating(modelBuilder);
    }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            AddDomainEventsAsOutboxMessages(); // this will make sure to load the domainEvent into outboxmessages and add them to the change tracker

            var result = await base.SaveChangesAsync(cancellationToken);
            // once we call SaveChangesAsync() we will prestiting everything into the database in a single transaction. which gives us atomic
            // grantees because we are using the sql database.
            // So either all of the outbox messages are presisted together as part of our transaction or nothing is peristed.

            return result;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyException("Concurrency exception occurred.", ex);
            // the reason for creating a custom exception is so that we don't leak ef details into my application layer.
            // we are abstracting ef error behind this custom exception. we are passing exception instance as an inner exception. so it is avilable for logging
            // and further inspecting.
        }
    }

    // instead of publishing domainEvent which are unreliable we want to convert them in outbox messages and store them into the database.
    // why unreliable because domainEvents can fail.

    private void AddDomainEventsAsOutboxMessages()
    {
        var outboxMessages = ChangeTracker.Entries<Entity>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                var domainEvents = entity.GetDomainEvents();

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .Select(domainEvent => new OutboxMessage(
                Guid.NewGuid(),
                _dateTimeProvider.UtcNow,
                domainEvent.GetType().Name,
                JsonConvert.SerializeObject(domainEvent, _jsonSerializerSettings)
                ))
            .ToList();

        // now add them to the change tracker. It will be presisted when we call saveChanges()

        AddRange(outboxMessages);
    }


}


/*

entity.ClearDomainEvents(); // This part is important. Because when we publish the domain events. We don't know what could be happening in the handlers
                // there could be another dbcontext created. which could use the same entity and add another domain event and it might cause strage behaviours.
                // It prevents duplicate publishing, maintain data consistency, Transactional integritity



 ######### There are some caviates(waringing) to this approach.

    var result = await base.SaveChangesAsync(cancellationToken);
await PublishDomainEventsAsync();


What we are doing here is potentially problematic.
we are saving changes to the database. which is atomic.This is either going to succeed or fail.


But then we are pulishing the set of domain events. Which is another transaction all together.

The domain event handlers could be doing all sorts of things like calling the database or other external services and
The handlers itself could also fail.This is going to cause an exception which is going to fail the saveChangesAsync method. However orignal call to the
database was already completed

later we will learn a better way of publishing domain events using the outbox pattern.


we are publishing domainEvents after persisting the changes in the database. and making sure db transaction has completed.


Lets think of 2 common senarios of handling the domainEvents

 1. When we are just talking with the databae : In which case we can wrap everything inside a database transaction. even the original changes
    persisted with this call
    var result = await base.SaveChangesAsync(cancellationToken);
    will be part of the same transaction.and here everything is fine.

    > The problem occures whenever you have a domainEvent handler that is also talking to the external services.

    > So what we want to do is detach the processing of the our main business transaction. which is contianed in this call.
        var result = await base.SaveChangesAsync(cancellationToken);

    > from any sideEffects that could include talking with the databases or other external services



# outbox message processing : we want to do this inside of a background job. This background job will pull the outbox messages and then publish the 
respective domain events.
*/