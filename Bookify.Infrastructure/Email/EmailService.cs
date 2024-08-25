using Bookify.Application.Abstractions.Email;

namespace Bookify.Infrastructure.Email;
internal sealed class EmailService : IEmailService
{
    public Task SendAsync(Domain.Users.Email recipient, string subject, string body)
    {
        // here we would implement email service

        // but for toturial he didn't wanted to do it. we just simulate that this method completed successfully.
        return Task.CompletedTask;
    }
}
