using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Authorization;
internal sealed class AuthorizationService
{
    private readonly ApplicationDbContext _dbcontext;

    public AuthorizationService(ApplicationDbContext context)
    {
        _dbcontext = context;
    }

    public async Task<UserRolesResponse> GetRolesForUserAsync(string identityId)
    {
        // this identityId will come from the 'name identifier claim'.


        var userRoles = await _dbcontext.Set<User>().Where(user => user.IdentityId == identityId)
            .Select(user => new UserRolesResponse(user.Id, user.Roles.ToList())).FirstOrDefaultAsync();

        return userRoles ?? throw new ApplicationException("User identity not found in database");

    }
}
