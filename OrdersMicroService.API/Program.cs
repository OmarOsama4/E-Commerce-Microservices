using BusinessLogicLayer;
using DataAccessLayer;
using FluentValidation.AspNetCore;
using OrdersMicroService.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

#region DI Services
builder.Services.AddBusinessLogicLayer();
builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddFluentValidationAutoValidation();
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
