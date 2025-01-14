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

        return userRoles ?? throw new ApplicationException("User roles not found.");

    }

    internal async Task<HashSet<string>> GetPermissionsForUserAsync(string identityId)
    {
        var permissions = await _dbcontext.Set<User>()
            .Where(user => user.IdentityId == identityId)
            .SelectMany(user => user.Roles.Select(role => role.Permissions))
            .FirstOrDefaultAsync();

        if (permissions is null)
        {
            throw new ApplicationException("Permissions not found.");
        }

        var permissionsSet = permissions.Select(p => p.Name).ToHashSet(); // to hashseet will get ride of any duplicate value.

        return permissionsSet;
    }
}
