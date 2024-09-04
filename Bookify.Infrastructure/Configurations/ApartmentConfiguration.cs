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