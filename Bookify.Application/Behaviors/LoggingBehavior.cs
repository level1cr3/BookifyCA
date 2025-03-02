using Bookify.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace Bookify.Application.Behaviors;
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IBaseRequest where TResponse : Result
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var name = request.GetType().Name; // using reflection to get name of the command

        try
        {
            _logger.LogInformation("Executing request {Request}", name);

            var result = await next();

            if (result.IsSuccess)
            {
                _logger.LogInformation("Request {Request} processed successfully", name);
            }
            else
            {
                //_logger.LogError("Request {Request} processed with {@Error}", name, result.Error); // to structure error as json object

                // using logcontext to push errors in error property
                using (LogContext.PushProperty("Error", result.Error, true))
                {
                    _logger.LogError("Request {Request} processed with error", name); 
                }
            }

            return result;
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Request {Request} processing failed", name);

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