using BusinessLogicLayer.Contract;
using BusinessLogicLayer.Mappers;
using BusinessLogicLayer.ServiceImplementation;
using BusinessLogicLayer.Validator;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogicLayer;
public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
    {

        services.AddValidatorsFromAssemblyContaining<OrderAddRequestValidator>();
        services.AddAutoMapper(cfg => 
        cfg.AddMaps(typeof(OrderAddRequestToOrderProfile).Assembly)
        );
        
        services.AddScoped<IOrdersService, OrdersService>();
        return services;
    }
}
