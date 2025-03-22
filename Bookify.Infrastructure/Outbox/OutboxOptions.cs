namespace Bookify.Infrastructure.Outbox;

internal class OutboxOptions
{
    public int IntervalInSeconds { get; init; } // this represents how often we want to run our background job.

    public int BatchSize { get; init; } // this represents how many outbox messages we are going to read.
                                        // in one single run of backgroud job and then publish them all one by one.
                                        // we will add this from appsettings.json configuration.
}
