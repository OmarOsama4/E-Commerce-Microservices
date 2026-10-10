using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace BusinessLogicLayer.RabbitMQ;
public class RabbitMQPublisher : IRabbitMQPublisher, IDisposable
{
    private readonly IConfiguration _configuration;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMQPublisher(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    public async Task InitializeConnectionAsync()
    {
        string hostName = _configuration["RabbitMQ_HostName"]!;
        string userName = _configuration["RabbitMQ_UserName"]!;
        string password = _configuration["RabbitMQ_Password"]!;
        int port = Convert.ToInt32(_configuration["RabbitMQ_Port"]);

        var connectionFactory = new ConnectionFactory
        {
            HostName = hostName,
            UserName = userName,
            Password = password,
            Port = port
        };

        _connection = await connectionFactory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
    }

    public void Publish<T>(string routingKey, T message)
    {
        throw new NotImplementedException();
    }
    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
