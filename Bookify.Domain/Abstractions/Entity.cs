namespace Bookify.Domain.Abstractions;

public abstract class Entity(Guid id)
{
    public Guid Id { get; init; } = id;

}


// what is Entity ?
// it is a object that has a unique identifier id. and it contineous that means existence of this object is important throught
// the life of the application