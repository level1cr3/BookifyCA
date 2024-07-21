using Bookify.Domain.Abstractions;

namespace Bookify.Domain.Apartments;

public sealed class Apartment(Guid id,
                              Name name,
                              Description description,
                              Address address,
                              Money price,
                              Money cleaningFee,
                              List<Amenity> amenities) : Entity(id)
{
    public Name Name { get; private set; } = name;

    public Description Description { get; private set; } = description;

    public Address Address { get; private set; } = address;

    public Money Price { get; private set; } = price;

    public Money CleaningFee { get; private set; } = cleaningFee;

    public List<Amenity> Amenities { get; private set; } = amenities;

    public DateTime? LastBookedOnUtc { get; private set; }
}



// The problem with strings or any premitive type which are part of your domain model is that they convey no meaning.
// To solve premitive obsession and also improve the design of our domain model we are going to introduce value object to represent the address.
// how we going to define the value objects. One that is really practical is using 'record type'.


// Why do we have private setters on all of th properties ?
// We don't want to let anyone change the values inside our entity from outside. If they could,
// it would break the rules and protections we've put in place to keep our design safe and consistent.