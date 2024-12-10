namespace Bookify.Infrastructure.Authentication;

public sealed class KeycloakOptions
{
    public string AdminUrl { get; init; } = string.Empty; // url to the admin api.

    public string TokenUrl { get; init; } = string.Empty; // token url which we will use later to obtain json web token for our users credential.

    public string AdminClientId { get; init; } = string.Empty; // client info for admin

    public string AdminClientSecret { get; init; } = string.Empty;

    public string AuthClientId { get; init; } = string.Empty; // to authenticate the clinet.
    
    public string AuthClientSecret { get; init; } = string.Empty;

}