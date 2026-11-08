using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace BusinessLogicLayer.RabbitMQ;
public class RabbitMQProductUpdateNameConsumer : IRabbitMQProductUpdateNameConsumer
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<RabbitMQProductUpdateNameConsumer> _logger;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMQProductUpdateNameConsumer(IConfiguration configuration, ILogger<RabbitMQProductUpdateNameConsumer> logger)
    {
        _configuration = configuration;
        _logger = logger;
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

    public async Task ConsumeAsync()
    {
        await InitializeConnectionAsync();
        string BindingKey = "product.update.name";
        string queueName = "orders.product.update.name.queue";

        //Create Exchange 
        var exchangeName = _configuration["RabbitMQ_Products_Exchange"];
        await _channel!.ExchangeDeclareAsync(
            exchange: exchangeName!,
            type: ExchangeType.Direct,
            durable: true
            );

        //Create queue
        await _channel!.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
            );

        //Bind the message to exchange
        await _channel.QueueBindAsync(
            queue: queueName,
            exchange: exchangeName!,
            routingKey: BindingKey);

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (sender, arg) =>
        {
            var body = arg.Body.ToArray();
            string message = Encoding.UTF8.GetString(body);

            if (!string.IsNullOrEmpty(message))
            {
                var productNameUpdateMessage = JsonSerializer.Deserialize<ProductNameUpdateMessage>(message);

                if (productNameUpdateMessage is not null)
                {
                    _logger.LogInformation(
                        $"Product name updated: {productNameUpdateMessage.ProductID}, " +
                        $"New name: {productNameUpdateMessage.NewName}");
                }
            }
        };

        await _channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: true,
            consumer: consumer);
    }
    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}