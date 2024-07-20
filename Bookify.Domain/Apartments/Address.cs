namespace Bookify.Domain.Apartments;

public record Address(string Country,
                      string State,
                      string ZipCode,
                      string City,
                      string Street);


// Why record is a good choice for implementing value object?
// What is a value object in domain driven design?
// > value object is uniquely identified by it's value. which means that it has structural equality. which is one of the feature that record support.
// in record if 2 record have same value then they are considered same.
// another quality that we are looking for in value objects is immutability. and record also satisfies that

// there implementation where they define base valueObject and then we have implement what are equality member so on. that is verbos stick to record.

// a great candidate for value object is anytime you see string. what is string ? what is a string with one character mean we can't know that