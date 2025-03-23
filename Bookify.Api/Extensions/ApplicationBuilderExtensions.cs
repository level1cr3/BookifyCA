using Bookify.Api.Endpoints.Bookings;
using Bookify.Api.Middleware;
using Bookify.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); // use scope to resolve the dbcontext.

        dbContext.Database.Migrate();
    }

    public static void UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }

    public static void UserRequestContextLogging(this IApplicationBuilder app) 
    {
        app.UseMiddleware<RequestContextLoggingMiddleware>();    
    }


    //public static void MapEndpoints(this WebApplication app) 
    //{
    //    var apiVersionSet = app.NewApiVersionSet().HasApiVersion(new Asp.Versioning.ApiVersion(1)).ReportApiVersions().Build();

    //    var routeGroupBuilder = app.MapGroup("api/v{version:apiVersion}").WithApiVersionSet(apiVersionSet);// we can add prefix for all endpoints created inside this group.
    //                                                                                                       // we could also make call to require authentication to make all enpoints require authentication.

    //    routeGroupBuilder.MapBookingEnpoints();

    //}


}

// This method ApplyMigrations is going to be used only for local development process.