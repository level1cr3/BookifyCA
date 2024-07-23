namespace Bookify.Domain.Abstractions;

public abstract class Entity(Guid id)
{
    private readonly List<IDomainEvent> _domainEvents = []; // it will contain domain events that are raised on this entity instance.

    public Guid Id { get; init; } = id;

    public IReadOnlyList<IDomainEvent> GetDomainEvents() => _domainEvents;

    public void  ClearDomainEvents() => _domainEvents.Clear();

    protected void RaiseDomainEvents(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);

    // we want to be able to raise domain event when something happens in the domain layer.

    // later we will learn how to publish these events using ef core and mediatR
}


// what is Entity ?
// it is a object that has a unique identifier id. and it contineous that means existence of this object is important throught
// the life of the application

// is uniquely identified by its identity. or id