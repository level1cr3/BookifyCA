using Bookify.Domain.Abstractions;
using MediatR;

namespace Bookify.Application.Abstractions.Messaging;
public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
    // essentially my query is going to be mediatr request. Returing a Result of TResponse object. query can either succeede or fail and result object should communicate that.


}
