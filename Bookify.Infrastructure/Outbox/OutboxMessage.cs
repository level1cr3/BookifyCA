namespace Bookify.Infrastructure.Outbox;

public sealed class OutboxMessage
{
    public OutboxMessage(Guid id, DateTime occurredOnUtc, string type, string content)
    {
        Id = id;
        OccurredOnUtc = occurredOnUtc;
        Type = type;
        Content = content;
    }

    public Guid Id { get; init; }

    public DateTime OccurredOnUtc { get; init; }

    public string Type { get; init; } // This represents the fully qualified name of the DomainEvent. That going to be serialized into an outbox message.

    public string Content { get; init; } // It is going to be JSON string representing my DomainEvent instance.

    public DateTime? ProcessedOnUtc { get; init; } // we will use this column to determine if the outbox messsage has been processed or not.
    //It it wasn't processed we are going to handler it using a bacground worker

    public string? Error { get; init; }

}

// This is going to be single row inside my outbox table.
