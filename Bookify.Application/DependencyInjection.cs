using Bookify.Application.Behaviors;
using Bookify.Domain.Bookings;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Bookify.Application;
public static class DependencyInjection
{
    // will be responsible for registering the services specific to application layer.

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(config =>
        {
            // to wire up command, command hanler and query , query handler
            config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly); // this assembly would be application project.

            config.AddOpenBehavior(typeof(LoggingBehavior<,>));

            config.AddOpenBehavior(typeof(QueryCachingBehavior<,>));

            config.AddOpenBehavior(typeof(ValidationBehavior<,>));

        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly); // this is the reason why we installed the specific fluent library. instead of installing a generic one.

        // here assebmly is 'application assembly aka my project name'. this will scan the assembly and register any validators as an IValidator instance which we are
        // using in validation behaviour.


        services.AddTransient<PricingService>();

        return services;
    }


    // we will call this method in API project. to configure services for application project.
}
