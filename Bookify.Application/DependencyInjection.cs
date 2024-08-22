using Bookify.Application.Behaviors;
using Bookify.Domain.Bookings;
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
        });


        services.AddTransient<PricingService>();

        return services;
    }


    // we will call this method in API project. to configure services for application project.
}
