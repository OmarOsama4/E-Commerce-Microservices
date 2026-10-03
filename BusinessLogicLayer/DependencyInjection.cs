using BusinessLogicLayer.Validator;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessLogicLayer;
public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogicLayer(this IServiceCollection services)
    {

        services.AddValidatorsFromAssemblyContaining<OrderAddRequestValidator>();

        return services;
    }
}
