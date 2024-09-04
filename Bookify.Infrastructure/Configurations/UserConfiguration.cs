using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;
internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(user => user.Id);

        builder.Property(user => user.FirstName)
            .HasMaxLength(200)
            .HasConversion(firstName => firstName.Value, value => new FirstName(value));

        builder.Property(user => user.LastName)
            .HasMaxLength(200)
            .HasConversion(lastName => lastName.Value, value => new LastName(value));

        builder.HasIndex(user => user.Email).IsUnique();

        // we are defining index in email column of the database and we are saying it is a unique index.
        // this will give us a database garanteed constraint that email is going to be unique accross the users table.
        // we are making email as non clustered index.


    }
}
