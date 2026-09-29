using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace eCommerce.Infrastructure.DbContext;
public class DapperDbContext
{
    private readonly IDbConnection connection;
    public DapperDbContext(IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("PostgresConnection") ?? throw new InvalidOperationException("No Connection string found");
        connection =  new NpgsqlConnection(connectionString);
    }

    public IDbConnection DbConnection => connection;
}
