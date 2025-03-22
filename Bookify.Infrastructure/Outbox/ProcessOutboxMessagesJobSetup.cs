using Microsoft.Extensions.Options;
using Quartz;

namespace Bookify.Infrastructure.Outbox;
internal class ProcessOutboxMessagesJobSetup : IConfigureOptions<QuartzOptions>
{
    private readonly OutboxOptions _outboxOptions; // we need interval in secound to configure how often we run ProcessOutboxMessagesJob

    public ProcessOutboxMessagesJobSetup(IOptions<OutboxOptions> outboxOptions)
    {
        _outboxOptions = outboxOptions.Value;
    }

    public void Configure(QuartzOptions options)
    {
        const string jobName = nameof(ProcessOutboxMessagesJob);

        options.AddJob<ProcessOutboxMessagesJob>(configure => configure.WithIdentity(jobName))
            .AddTrigger(configure => 
                configure
                .ForJob(jobName)
                .WithSimpleSchedule(schedule => 
                    schedule.WithIntervalInSeconds(_outboxOptions.IntervalInSeconds).RepeatForever())
                );

    }
}


//  to tell quartz to run ProcessOutboxMessagesJob