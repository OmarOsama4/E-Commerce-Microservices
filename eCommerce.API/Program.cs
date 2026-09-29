using eCommerce.API.Middlewares;
using eCommerce.Core;
using eCommerce.Core.Mappers;
using eCommerce.Infrastructure;
using FluentValidation.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

//Add Services
builder.Services.AddInfrastructure();
builder.Services.AddCore();

builder.Services.AddControllers().AddJsonOptions(
    opt => opt.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
    );

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddMaps(typeof(ApplicationUserMappingProfile).Assembly);
});

builder.Services.AddFluentValidationAutoValidation();

//Build the Web App 
var app = builder.Build();

app.UseExceptionHandlingMiddleware();

//Routing
app.UseRouting();

//Auth
app.UseAuthentication();
app.UseAuthorization();

//Controller Routes
app.MapControllers();

app.Run();
