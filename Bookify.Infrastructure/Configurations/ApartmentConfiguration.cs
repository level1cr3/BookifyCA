using Bookify.Domain.Apartments;
using Bookify.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;
internal sealed class ApartmentConfiguration : IEntityTypeConfiguration<Apartment>
{
    public void Configure(EntityTypeBuilder<Apartment> builder)
    {
        builder.ToTable("apartments");

        builder.HasKey(apartment => apartment.Id);

        builder.OwnsOne(apartment => apartment.Address, addressBuilder =>
        {
            addressBuilder.Property(address => address.Country).HasMaxLength(100);
            addressBuilder.Property(address => address.State).HasMaxLength(100);
            addressBuilder.Property(address => address.City).HasMaxLength(100);
            addressBuilder.Property(address => address.Street).HasMaxLength(255);
            addressBuilder.Property(address => address.ZipCode).HasMaxLength(20);
        });

        builder.Property(apartment => apartment.Name)
               .HasMaxLength(200)
               .HasConversion(name => name.Value, value => new Name(value));

        // we will define simple conversion from value object to the premitive type in the database. 
        // and from premitive type back into the value object.

        builder.Property(apartment => apartment.Description)
            .HasMaxLength(2000)
            .HasConversion(description => description.Value, value => new Description(value));

        builder.OwnsOne(apartment => apartment.Price, priceBuilder =>
        {
            priceBuilder.Property(money => money.Currency)
            .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
        }); // we are only mapping currency code to the database


        builder.OwnsOne(apartment => apartment.CleaningFee, priceBuilder =>
        {
            priceBuilder.Property(money => money.Currency)
            .HasConversion(currency => currency.Code, code => Currency.FromCode(code));
        });


        builder.Property<uint>("Version").IsRowVersion();
        // we are defining a shadow property on apartment entity.
        // postgress will use system column which is the xmin column and it holds the value of last updating transaction.

    }

}



// we need to connect our domain entity with ef core.
// currently we have defined them in persistent ignorant way.
// We designed a rich domain model and how are we going to map this to database is by using ef core fluent configurations. 


// ##### OwnsOne explaination.

// builder.OwnsOne(apartment => apartment.Address)

// we are mapping our Address value object. which is complex object with few properties. using an owned enitity
// how this works with ef core is value object is going to be mapped with set of columns in the same table as the owning entity.
// So in this case address columns are going to be in apartments table.

// if this is a collection. we have support for calling 'ownsmany' method for mapping collection of value objects. But in that case the owned collection is 
// going to be mapped into a seprate table.





// 
//Optimistic concurrency : is a mechanism tha relias on "having some column in the database acting as the version for that row".
//Usually this is a database generated value and it is used when persisting changes to the database. To check if the version that is currently in database
//is different from the one that we have in our application at the time when we loaded the entity. If those versions are not a match it means that somebody has 
//changed this row in the database before we could and we throw a database concurrency exception.