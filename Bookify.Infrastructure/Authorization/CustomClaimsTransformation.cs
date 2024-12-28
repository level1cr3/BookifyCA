using Bookify.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;

namespace Bookify.Infrastructure.Authorization;
internal sealed class CustomClaimsTransformation : IClaimsTransformation
{
    private readonly IServiceProvider _serviceProvider; // we will use this to create the service scoped.

    public CustomClaimsTransformation(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.HasClaim(claim => claim.Type == ClaimTypes.Role) && principal.HasClaim(claim => claim.Type == JwtRegisteredClaimNames.Sub))
        {
            return principal;
        }

        // we want to fetch the roles and user identifier. for the currently authenticated user and add them as claims in our claims principal obj.


        //using var scope = _serviceProvider.CreateScope(); 

        await using var scope = _serviceProvider.CreateAsyncScope(); // using this because within the scope i'm callling the asyncronous method. GetRolesForUserAsync

        var authorizationService = scope.ServiceProvider.GetRequiredService<AuthorizationService>();

        var IdentityId = principal.GetIdentityId();

        var userRoles = await authorizationService.GetRolesForUserAsync(IdentityId);


        // using pattern matching. to cast identity to claimsIdentity
        if (principal.Identity is not ClaimsIdentity claimsIdentity)
        {
            throw new InvalidOperationException("The principal's identity must be a ClaimsIdentity");
        }

        claimsIdentity.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, userRoles.Id.ToString()));

        foreach (var role in userRoles.Roles)
        {
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role.Name));

        }

        return principal;   
    }
}

//ClaimTypes.Role this is the claim  type that expected by the "authorize" attribute in our endpoints
// JwtRegisteredClaimNames.Sub  this is for implementing resource base authorization. The reason we are looking for 'sub' claim is that asp.net core
// is going to transform 'incoming sub claim from keycloack' into the 'name identifier' claim. so the sub claim itself won't be there.