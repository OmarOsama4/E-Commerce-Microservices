using BusinessLogicLayer.Contract;
using BusinessLogicLayer.Mappers;
using BusinessLogicLayer.RabbitMQ;
using BusinessLogicLayer.ServiceImplementation;
using BusinessLogicLayer.Validator;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogicLayer;
public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddValidatorsFromAssemblyContaining<OrderAddRequestValidator>();
        services.AddAutoMapper(cfg => 
        cfg.AddMaps(typeof(OrderAddRequestToOrderProfile).Assembly)
        );

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = $"{configuration["REDIS_HOST"]}:{configuration["REDIS_PORT"]}";
        });
        
        services.AddScoped<IOrdersService, OrdersService>();
        services.AddTransient<IRabbitMQProductUpdateNameConsumer, RabbitMQProductUpdateNameConsumer>();
        services.AddHostedService<RabbitMQProductUpdateHostedService>();
        return services;
    }
}
