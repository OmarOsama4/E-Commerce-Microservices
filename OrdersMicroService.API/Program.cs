using BusinessLogicLayer;
using BusinessLogicLayer.HttpClients;
using DataAccessLayer;
using FluentValidation.AspNetCore;
using OrdersMicroService.API.Middlewares;
using Polly;

var builder = WebApplication.CreateBuilder(args);

#region DI Services
builder.Services.AddBusinessLogicLayer();
builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddControllers();

builder.Services.AddHttpClient<UsersMicroserviceClient>(client =>
{
    client.BaseAddress = new Uri($"http://{builder.Configuration["UsersMicroserviceName"]}:" +
        $"{builder.Configuration["UsersMicroservicePort"]}");
}).AddPolicyHandler(
        Policy.HandleResult<HttpResponseMessage>( r=> !r.IsSuccessStatusCode)
        .WaitAndRetryAsync(retryCount:5, sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(1),
        onRetry:(outcome, timespan, retryAttempts, context) => {  }));


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

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
#endregion

app.Run();
