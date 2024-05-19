using AwesomeFiles.Api.Extensions;
using AwesomeFiles.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

builder.Services
    .AddApiLogging()
    .AddApplicationServices()
    .AddCustomMiddlewares();

builder.ConfigureArchiveStorage();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app
    .UseLoggingRequests()
    .UseExceptionHandling();

app.MapControllers();

app.Run();