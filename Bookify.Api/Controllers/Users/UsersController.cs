using Asp.Versioning;
using Bookify.Application.Users.GetLoggedInUser;
using Bookify.Application.Users.LogInUser;
using Bookify.Application.Users.RegisterUser;
using Bookify.Infrastructure.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Bookify.Api.Controllers.Users;

[ApiController]
[ApiVersion(ApiVersions.V1)]
//[ApiVersion(ApiVersions.V2)]
//[ApiVersion(ApiVersions.V2, Deprecated = true)] // it allows me configure supported versions in this controller.
[Route("api/v{version:apiVersion}/user")] // now this allows us to specify the api path using v1 or v2.
public class UsersController : ControllerBase
{
    private readonly ISender _sender;

    public UsersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("me")]
    [HasPermission(Permissions.UsersRead)]
    public async Task<IActionResult> GetLoggedInUser(CancellationToken cancellationToken)
    {
        var query = new GetLoggedInUserQuery();

        var result = await _sender.Send(query, cancellationToken);

        return Ok(result.Value);
    }


    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(request.Email,
                                              request.FirstName,
                                              request.LastName,
                                              request.Password);


        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return BadRequest(result.Error);
        }

        return Ok(result.Value);

    }


    [AllowAnonymous]
    //[MapToApiVersion(ApiVersions.V1)]
    [HttpPost("login")]
    public async Task<IActionResult> LogIn(LogInUserRequest request, CancellationToken cancellationToken)
    {
        var command = new LogInUserCommand(request.Email, request.Password);

        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return Unauthorized(result.Error);
        }

        return Ok(result.Value);

    }


    //[AllowAnonymous]
    //[MapToApiVersion(ApiVersions.V2)]
    //[HttpPost("login")]
    //public async Task<IActionResult> LogInV2(LogInUserRequest request, CancellationToken cancellationToken)
    //{
    //    var command = new LogInUserCommand(request.Email, request.Password);

    //    var result = await _sender.Send(command, cancellationToken);

    //    if (result.IsFailure)
    //    {
    //        return Unauthorized(result.Error);
    //    }

    //    return Ok(result.Value);

    //}


}
