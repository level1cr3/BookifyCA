using Bookify.Api.Extensions;
using Bookify.Application;
using Bookify.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// this is all we need to wireup dependency inejction. because we added the respective service registrations in the application and infrastructure project.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);


var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.ApplyMigrations(); // only inside the development.
    //app.SeedData(); // run only once.
}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
