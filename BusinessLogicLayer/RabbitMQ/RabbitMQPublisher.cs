using Microsoft.Extensions.Configuration;

namespace BusinessLogicLayer.RabbitMQ;
public class RabbitMQPublisher(IConfiguration configuration) : IRabbitMQPublisher
{
    public void Publish<T>(string routingKey, T message)
    {
        throw new NotImplementedException();
    }
}
