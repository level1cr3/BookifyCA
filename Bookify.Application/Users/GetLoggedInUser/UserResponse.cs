namespace Bookify.Application.Users.GetLoggedInUser;
public sealed record UserResponse(Guid Id, string Email, string Name);
