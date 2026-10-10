using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

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
        if (_connection is not null && _channel is not null)
            return;

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

    public async Task Publish<T>(string routingKey, T message)
    {
        await InitializeConnectionAsync();
        string messageJson = JsonSerializer.Serialize(message);
        var messageInBytes = Encoding.UTF8.GetBytes(messageJson);

        //Create Exchange 
        await _channel!.ExchangeDeclareAsync(
            exchange: "products.exchange",
            type: ExchangeType.Direct,
            durable: true
            );

        //Publish Message
        await _channel!.BasicPublishAsync(
            exchange: "products.exchange",
            routingKey: routingKey,
            body: messageInBytes
            );
    }
    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
