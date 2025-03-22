using Bookify.Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bookify.Infrastructure.Configurations;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("outbox_messages");

        builder.HasKey(outboxMessage => outboxMessage.Id);

        builder.Property(outboxMessage => outboxMessage.Content).HasColumnType("jsonb"); // becaues we are using postgresql we have access for postgress support for json column
        //jsonb is short for binary json. which is more powerfull json format in postgres. If your db doesn't support it we could just use the nvarchar


        // we don't have to configure all the property. only the ones that require configuring. like telling what name to use. what columntype ? their length and how to convert them back and so on...
    }
}
