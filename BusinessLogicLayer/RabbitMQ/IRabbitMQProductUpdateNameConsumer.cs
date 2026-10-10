namespace BusinessLogicLayer.RabbitMQ;
public interface IRabbitMQProductUpdateNameConsumer
{
    Task ConsumeAsync();
}