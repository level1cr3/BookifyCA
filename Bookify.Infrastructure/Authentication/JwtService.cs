﻿using Bookify.Application.Abstractions.Authentication;
using Bookify.Domain.Abstractions;
using Bookify.Infrastructure.Authentication.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Bookify.Infrastructure.Authentication;
internal class JwtService : IJwtService
{
    private static readonly Error AuthenticationFailed = new("Keycloak.AuthenticationFailed", "Failed to acquire access token due to authentication failure");

    private readonly HttpClient _httpClient;
    private readonly KeycloakOptions _keycloakOptions;

    public JwtService(HttpClient httpClient, IOptions<KeycloakOptions> keycloakOptions)
    {
        _httpClient = httpClient;
        _keycloakOptions = keycloakOptions.Value;
    }




    public async Task<Result<string>> GetAccessTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        try
        {

            KeyValuePair<string, string>[] authRequestParams =
            [
                new("client_id", _keycloakOptions.AuthClientId),
                new("client_secret", _keycloakOptions.AuthClientSecret),
                new("scope", "openid email"),
                new("grant_type", "password"),
                new("username", email),
                new("password", password),
            ];
             
            var authorizationRequestContent = new FormUrlEncodedContent(authRequestParams);

            var response = await _httpClient.PostAsync("",authorizationRequestContent,cancellationToken);

            response.EnsureSuccessStatusCode();

            var authorizationToken = await response.Content.ReadFromJsonAsync<AuthorizationToken>(cancellationToken);

            if (authorizationToken is null)
            {
                return Result.Failure<string>(AuthenticationFailed);
            }

            return authorizationToken.AccessToken;

        }
        catch (HttpRequestException)
        {
            return Result.Failure<string>(AuthenticationFailed);
        }
    }


}
