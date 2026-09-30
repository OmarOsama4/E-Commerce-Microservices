using BusinessLogicLayer.Mappers;
using BusinessLogicLayer.ServiceContract;
using BusinessLogicLayer.ServiceImplementation;
using BusinessLogicLayer.Validatos;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogicLayer;
public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
        {
            cfg.AddMaps(typeof(ProductToProductResponse).Assembly);
        });
        services.AddScoped<IProductService, ProductService>();
        services.AddValidatorsFromAssemblyContaining<ProductAddRequestValidator>();
        return services;
    }
}