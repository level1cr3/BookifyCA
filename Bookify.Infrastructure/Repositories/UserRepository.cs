using Bookify.Domain.Users;

namespace Bookify.Infrastructure.Repositories;
internal sealed class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public override void Add(User user)
    {
        foreach (var role in user.Roles)
        {
            DbContext.Attach(role); // this will tell ef core any role present in our user obj. is already there in the db.
        }

        DbContext.Add(user);
    }
}


// This is ideal way to use the repository pattern. In your domain layer you will define specific repository interfaces
// containing only the methods you need for that entity and in the infrastructure layer. We can leverage the generic repository pattern
// to reduce repeative code
