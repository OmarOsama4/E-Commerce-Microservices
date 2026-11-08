namespace BusinessLogicLayer.RabbitMQ
{
    public interface IRabbitMQProductDeletionConsumer
    {
        Task ConsumeAsync();
        void Dispose();
    }
}