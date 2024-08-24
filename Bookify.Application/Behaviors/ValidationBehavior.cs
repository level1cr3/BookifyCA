using Bookify.Application.Exceptions;
using FluentValidation;
using MediatR;

namespace Bookify.Application.Behaviors;
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IBaseRequest
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);


        List<ValidationError> validationErrors = _validators.Select(validator => validator.Validate(context))
                                          .Where(validationResult => validationResult.Errors.Count > 0)
                                          .SelectMany(validationResult => validationResult.Errors)
                                          .Select(validationFailure => new ValidationError(validationFailure.PropertyName, validationFailure.ErrorMessage))
                                          .ToList();


        if (validationErrors.Count > 0)
        {
            throw new Exceptions.ValidationException(validationErrors);
            // validation is good case for custom exception class.

            // we would be handling this in some of the upper layers of the architecture. A good candidate is exception handling middleware in the api layer.
        }

        return await next();
    }


}
