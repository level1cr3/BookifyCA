using Bookify.Application.Abstractions.Authentication;
using Bookify.Application.Abstractions.Messaging;
using Bookify.Domain.Abstractions;
using Bookify.Domain.Users;

namespace Bookify.Application.Users.RegisterUser;

internal sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserCommandHandler(IAuthenticationService authenticationService, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _authenticationService = authenticationService;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = User.Create(new FirstName(request.FirstName),
                               new LastName(request.LastName),
                               new Email(request.Email));

        var identityId = await _authenticationService.RegisterAsync(user, request.Password, cancellationToken);

        user.SetIdentityId(identityId);

        _userRepository.Add(user);

        await _unitOfWork.SaveChangesAsync();

        return user.Id;
    }


}


// 2 aspects that are important for our implementation is that.
// 1. we are not storing the roles. as part of our identity provider. which in this case is keycloak acting as the external service.
// we are using keycloak as a service to only verify who the current user is.

// we are using keycloak for authentication. and we are going to implement the support for the authorization as part of our system. after we have already
// authenticated the user. this approach gives us more control over the authorization aspect and It also improves the flexibility of our system.


// 2. 2nd aspect that. is that we are using ef core under the hood. when we call the add method to insert the user. It will take the role. and try to insert 
// in the role table. because we are sending the role as object with the user obj. Which will cause the duplicate key exception.
// to fix this we will make the add method in base repository as virtual and override that method from the user repository and tell the db that these.
// roles are already attached to the Dbcontext.

