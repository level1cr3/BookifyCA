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
}

// This method ApplyMigrations is going to be used only for local development process.