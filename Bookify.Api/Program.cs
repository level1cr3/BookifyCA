using Bookify.Api.Endpoints.Bookings;
using Bookify.Api.Extensions;
using Bookify.Api.OpenApi;
using Bookify.Application;
using Bookify.Infrastructure;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Host.UseSerilog((context, configure) => configure.ReadFrom.Configuration(context.Configuration));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// this is all we need to wireup dependency inejction. because we added the respective service registrations in the application and infrastructure project.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    // passing the delegate
    app.UseSwaggerUI(options =>
    {
        var descriptions = app.DescribeApiVersions(); // using web application to call the  describeapiversion

        foreach (var description in descriptions)
        {
            var url = $"/swagger/{description.GroupName}/swagger.json";
            var name = description.GroupName.ToUpperInvariant();
            options.SwaggerEndpoint(url, name);
        }
    });

    app.ApplyMigrations(); // only inside the development.
    //app.SeedData(); // run only once.
}

app.UseHttpsRedirection();

app.UserRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseCustomExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

//app.MapEndpoints(); // could create extensioin method for including all the endpoints.


var apiVersionSet = app.NewApiVersionSet().HasApiVersion(new Asp.Versioning.ApiVersion(1)).ReportApiVersions().Build();

var routeGroupBuilder = app.MapGroup("api/v{version:apiVersion}").WithApiVersionSet(apiVersionSet);// we can add prefix for all endpoints created inside this group.
// we could also make call to require authentication to make all enpoints require authentication.

routeGroupBuilder.MapBookingEnpoints();


app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
}); // specify the endpoint where you want to expose the health

app.Run();