﻿using Bookify.Domain.Abstractions;
using Bookify.Domain.Users.Events;

namespace Bookify.Domain.Users;

public sealed class User : Entity
{
    private User(Guid id, FirstName firstName, LastName lastName, Email email)
        : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }

    private User()
    {
            
    }

    public FirstName FirstName { get; private set; }

    public LastName LastName { get; private set; }

    public Email Email { get; private set; }
    
    public string IdentityId { get; private set; } = string.Empty;

    public static User Create(FirstName firstName, LastName lastName, Email email)
    {
        var user = new User(Guid.NewGuid(), firstName, lastName, email);

        user.RaiseDomainEvents(new UserCreatedDomainEvent(user.Id));
        // now when we persist user in db. we are also going to publish user created domain event. someone can subscribe to this event and execute some behaviour asyncronously
        // ex: sending user welcome message when they register to bookify system.

        return user;
    }

    public void SetIdentityId(string identityId)
    {
        IdentityId = identityId;
    }
}


// what is the benifit of wrapping the constructor inside a factory method ?
// there are few benifits to using this approach
// 1. hiding your constructor which could have someother implementation details. that we don't to expose outside of the user entity
// 2. encapsulation.
// 3. to be able to introduce some side-effect inside the factory method that don't naturally don't belong inside the constructor.
// // what we are talking about is domain events

