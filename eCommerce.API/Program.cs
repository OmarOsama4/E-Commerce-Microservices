using eCommerce.API.Middlewares;
using eCommerce.Core;
using eCommerce.Core.Mappers;
using eCommerce.Infrastructure;
using FluentValidation.AspNetCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

#region Add Services
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
#endregion

//Build the Web App 
var app = builder.Build();

#region Add Middlewares
app.UseExceptionHandlingMiddleware();

//Routing
app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

//Auth
app.UseAuthentication();
app.UseAuthorization();

//Controller Routes
app.MapControllers(); 
#endregion

app.Run();
