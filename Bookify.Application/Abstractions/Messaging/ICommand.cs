using Bookify.Domain.Abstractions;
using MediatR;

namespace Bookify.Application.Abstractions.Messaging;
public interface ICommand : IRequest<Result>, IBaseCommand
{
}


public interface ICommand<TResponse> : IRequest<Result<TResponse>>, IBaseCommand
{
}


public interface IBaseCommand
{ 
    // value behind base command interface is that we can apply generic constraints in our pipeline behaviour. we see that when we implement the 
    // corss cutting concerns like logging and error logging.
}