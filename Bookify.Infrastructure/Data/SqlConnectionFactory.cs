using Bookify.Application.Abstractions.Data;
using Npgsql;
using System.Data;

namespace Bookify.Infrastructure.Data;
internal sealed class SqlConnectionFactory(string connectionString) : ISqlConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(connectionString);
        connection.Open();

        return connection;        
    }
}
