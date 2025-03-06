using Bookify.Application.Abstractions.Caching;
using Bookify.Domain.Users;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Infrastructure.Authorization;
internal sealed class AuthorizationService
{
    private readonly ApplicationDbContext _dbcontext;
    private readonly ICacheService _cacheService;

    public AuthorizationService(ApplicationDbContext context, ICacheService cacheService)
    {
        _dbcontext = context;
        _cacheService = cacheService;
    }

    public async Task<UserRolesResponse> GetRolesForUserAsync(string identityId)
    {
        // this identityId will come from the 'name identifier claim'.

        var cacheKey = $"auth:roles-{identityId}";

        var cachedRoles = await _cacheService.GetAsync<UserRolesResponse>(cacheKey);

        if (cachedRoles is not null)
        {
            return cachedRoles;
        }

        var userRoles = await _dbcontext.Set<User>().Where(user => user.IdentityId == identityId)
            .Select(user => new UserRolesResponse(user.Id, user.Roles.ToList())).FirstOrDefaultAsync();


        await _cacheService.SetAsync(cacheKey, userRoles);

        return userRoles ?? throw new ApplicationException("User roles not found.");
    }

    internal async Task<HashSet<string>> GetPermissionsForUserAsync(string identityId)
    {
        var cacheKey = $"auth:permissions-{identityId}";

        var cachedPermissions = await _cacheService.GetAsync<HashSet<string>>(cacheKey);

        if (cachedPermissions is not null)
        {
            return cachedPermissions;
        }


        var permissions = await _dbcontext.Set<User>()
            .Where(user => user.IdentityId == identityId)
            .SelectMany(user => user.Roles.Select(role => role.Permissions))
            .FirstOrDefaultAsync();

        if (permissions is null)
        {
            throw new ApplicationException("Permissions not found.");
        }

        var permissionsSet = permissions.Select(p => p.Name).ToHashSet(); // to hashseet will get ride of any duplicate value.

        await _cacheService.SetAsync(cacheKey, permissionsSet);

        return permissionsSet;
    }
}

// It is going to be expensive to query the users role and permissions in every authorization request. So that is the reason we will use the
// caching here.

// we are using cache aside pattern. first checking if there is value in cache. if not getting value from db and putting it in the cache.