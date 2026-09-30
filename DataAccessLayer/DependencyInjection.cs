using DataAccessLayer.Context;
using DataAccessLayer.Contracts;
using DataAccessLayer.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccessLayer;
public static class DependencyInjection
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(opt =>
        {
            opt.UseMySQL(configuration.GetConnectionString("DefaultConnection")!);
        });

        services.AddScoped<IProductRepository, ProductRepository>();

        return services;
    }
}