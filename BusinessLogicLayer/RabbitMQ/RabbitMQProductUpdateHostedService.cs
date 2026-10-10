using Microsoft.Extensions.Hosting;

namespace BusinessLogicLayer.RabbitMQ;
public class RabbitMQProductUpdateHostedService(IRabbitMQProductUpdateNameConsumer consumer) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await consumer.ConsumeAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        consumer.Dispose();
        return Task.CompletedTask;
    }
}
