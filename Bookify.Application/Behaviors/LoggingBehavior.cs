using Bookify.Application.Abstractions.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Bookify.Application.Behaviors;
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IBaseCommand
{
    private readonly ILogger<TRequest> _logger;

    public LoggingBehavior(ILogger<TRequest> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var name = request.GetType().Name; // using reflection to get name of the command

        try
        {
            _logger.LogInformation($"Executing command {name}");

            var result = await next();

            _logger.LogInformation($"Command {name} processed successfully");

            return result;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, $"Command {name} processing failed");

            throw;
        }
    }
}



// we are applying logging only for commands and not for query.
// we are using mediatr IPipelineBehavior for logging.

// we can enrich these logs. by providing the informations such as who is the user by giving userId, what is the Id of the command if we assign id's to the commands
// what is corelationId, requestId and so on.



//After configuring it in dependency injection. now when we send our commands. It will first go to logging behaviour run logging statement and then execute the command 
// handler before returning the response.