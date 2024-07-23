using MediatR;

namespace Bookify.Domain.Abstractions;
public interface IDomainEvent : INotification
{
}

// this interface is representing all of the domain events in our system.

// What is domain event ?
// Domain event is something of significant that has occured in the domain and you want to notify other components in your system.

// we will use mediatr to implement the domain events. mediatr contracts only contains the interfaces required for using mediatr
// mediatR notifications are used to implement the publish-subscribe pattern. we will be publishing the domain events and we could have 1 or more subscribers to
// this event that wants to handle it
