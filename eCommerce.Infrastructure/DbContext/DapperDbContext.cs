using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace eCommerce.Infrastructure.DbContext;
public class DapperDbContext
{
    private readonly IDbConnection connection;
    public DapperDbContext(IConfiguration configuration)
    {
        string? connectionStringTemplate = configuration.GetConnectionString("PostgresConnection")!;
        string connectionString = connectionStringTemplate
            .Replace("$POSTGRES_HOST", Environment.GetEnvironmentVariable("POSTGRES_HOST"))
            .Replace("$POSTGRES_PASSWORD", Environment.GetEnvironmentVariable("POSTGRES_PASSWORD"))
            .Replace("$POSTGRES_DATABASE", Environment.GetEnvironmentVariable("POSTGRES_DATABASE"))
            .Replace("$POSTGRES_USER", Environment.GetEnvironmentVariable("POSTGRES_USER"))
            .Replace("$POSTGRES_PORT", Environment.GetEnvironmentVariable("POSTGRES_PORT"));

        connection =  new NpgsqlConnection(connectionString);
    }

    public IDbConnection DbConnection => connection;
}
