using BusinessLogicLayer;
using BusinessLogicLayer.HttpClients;
using BusinessLogicLayer.PollyPolicies;
using DataAccessLayer;
using FluentValidation.AspNetCore;
using OrdersMicroService.API.Middlewares;
using Polly;

var builder = WebApplication.CreateBuilder(args);

#region DI Services
builder.Services.AddBusinessLogicLayer(builder.Configuration);
builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddControllers();

builder.Services.AddTransient<IPollyPolicies, PollyPolicies>();
builder.Services.AddTransient<IUsersMicroservicePolicies, UsersMicroservicePolicies>();

builder.Services.AddHttpClient<UsersMicroserviceClient>(client =>
{
    client.BaseAddress = new Uri($"http://{builder.Configuration["UsersMicroserviceName"]}:" +
        $"{builder.Configuration["UsersMicroservicePort"]}");
})
.AddPolicyHandler((serviceProvider, request) =>
{
    return serviceProvider
        .GetRequiredService<IUsersMicroservicePolicies>()
        .GetCombinedPolicy();
});


builder.Services.AddHttpClient<ProductsMicroserviceClient>(client =>
{
    client.BaseAddress = new Uri($"http://{builder.Configuration["ProductsMicroserviceName"]}:" +
        $"{builder.Configuration["ProductsMicroservicePort"]}");
});

#endregion

var app = builder.Build();

#region Application Pipeline
app.UseExceptionHandlingMiddleware();
app.UseRouting();

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
#endregion

app.Run();
