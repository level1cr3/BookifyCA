using Bookify.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Infrastructure.Authorization;


internal sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceProvider _serviceProvider;

    public PermissionAuthorizationHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
    {
        // we need to figure if user is authenticated and if they have required set of permissions.

        if (context.User.Identity is not { IsAuthenticated: true })
        {
            // checking if identity is not an object where isAuthenticated value is true.
            return;
        }

        // use authorization service to obtain the permission for the current user. then check if they satisfy the permission requirement.

        // we have to use _serviceProvider because we are goining serve authorization Handler as a traint service.

        await using var scope = _serviceProvider.CreateAsyncScope();

        var authorizationService = scope.ServiceProvider.GetRequiredService<AuthorizationService>();

        var identityId = context.User.GetIdentityId(); // getting identityId from authentication token.

        HashSet<string> permissions = await authorizationService.GetPermissionsForUserAsync(identityId);
        // disadvantage is we have to query the database everytime to obtain the set of permissions for the currently authenticated users
        // we can imporve this by introducing caching. In the authrozation service


        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }        

    }
}
