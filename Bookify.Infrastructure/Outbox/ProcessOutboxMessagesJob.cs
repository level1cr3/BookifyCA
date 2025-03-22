using Bookify.Application.Abstractions.Clock;
using Bookify.Application.Abstractions.Data;
using Bookify.Domain.Abstractions;
using Dapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;
using System.Data;

namespace Bookify.Infrastructure.Outbox;

[DisallowConcurrentExecution] // makes sure there is only one outbox messages instance job is running at a time. It will resolve any concurrency problem
internal sealed class ProcessOutboxMessagesJob : IJob
{
    private static readonly JsonSerializerSettings _jsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    }; // using this context because this is what we added in applicatoinDbContext when persisting data to the outboxmessage table.

    private readonly ISqlConnectionFactory _sqlConnectionFactory; // it will be used to query the outbox table. and update the processed messages using dapper
    private readonly IPublisher _publisher; // to publish individual domain events
    private readonly IDateTimeProvider _dateTimeProvider; // to record when outbox message was proccessed
    private readonly OutboxOptions _outboxOptions; // to get batch size
    private readonly ILogger<ProcessOutboxMessagesJob> _logger; // to log contextual informations

    public ProcessOutboxMessagesJob(
        ISqlConnectionFactory sqlConnectionFactory,
        IPublisher publisher,
        IDateTimeProvider dateTimeProvider,
        IOptions<OutboxOptions> outboxOptions,
        ILogger<ProcessOutboxMessagesJob> logger)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _publisher = publisher;
        _dateTimeProvider = dateTimeProvider;
        _outboxOptions = outboxOptions.Value;
        _logger = logger;
    }




    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Beginning to process outbox messages");

        using var connection = _sqlConnectionFactory.CreateConnection();
        using var transaction = connection.BeginTransaction();// because we want to process all of the outbox messages inside a single transaction. make sure they are proccessed together.

        var outboxMessages = await GetOutboxMessagesAsync(connection, transaction);

        foreach (var outboxMessage in outboxMessages)
        {
            Exception? exception = null;

            try
            {

                var domainEvent = JsonConvert.DeserializeObject<IDomainEvent>(outboxMessage.Content, _jsonSerializerSettings)!;

                await _publisher.Publish(domainEvent, context.CancellationToken);

            }
            catch (Exception caughtException)
            {
                _logger.LogError(
                    caughtException,
                    "Exception while processing outbox message {MessageId}",
                    outboxMessage.Id
                    );

                exception = caughtException;
            }


            await UpdateOutboxMessageAsync(connection, transaction, outboxMessage, exception);
        }


        transaction.Commit();

        _logger.LogInformation("Completed processing outbox messages");

    }



    private async Task<IReadOnlyCollection<OutboxMessageResponse>> GetOutboxMessagesAsync(IDbConnection connection, IDbTransaction transaction)
    {
        const string sql = $"""
            SELECT id, content
            FROM outbox_messages
            WHERE processed_on_utc IS NULL
            ORDER BY occurred_on_utc
            LIMIT @Limit
            FOR UPDATE
            """;  

        // "FOR UPDATE" : will lock any rows that are queried as part of db transaction. Until we commit that transaction.
        //  This means if there are competing transaction for multiple instances of your background job running.
        //  theses rows would be locked and concurrent instance of processoutbox messages job won't be able to read thoes rows
        // which is useful because you only want to proccesses your outbox messages only once. which why we have also added. "WHERE processed_on_utc IS NULL"

        var outboxMessages = await connection.QueryAsync<OutboxMessageResponse>(sql, new { Limit = _outboxOptions.BatchSize }, transaction: transaction);

        return [.. outboxMessages];
    }


    private async Task UpdateOutboxMessageAsync(IDbConnection connection, IDbTransaction transaction, OutboxMessageResponse outboxMessage, Exception? exception)
    {
        const string sql = """
            UPDATE outbox_messages
            SET processed_on_utc = @ProcessedOnUtc
                error = @Error
            WHERE id = @Id
            """;

        await connection.ExecuteAsync(
            sql,
            new
            {
                outboxMessage.Id,
                ProcessedOnUtc = _dateTimeProvider.UtcNow,
                Error = exception?.ToString()
            }, 
            transaction: transaction);
    }


    internal sealed record OutboxMessageResponse(Guid Id, string Content);


}

// we are gonna use Quartz library for the background jobs