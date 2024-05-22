using AwesomeFiles.Api.Extensions;
using AwesomeFiles.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();
var configuration = builder.Configuration;

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

builder.Services
    .AddApiLogging(configuration)
    .AddApplicationServices(configuration)
    .AddCustomMiddlewares()
    .AddCaching(configuration);

builder.ConfigureArchiveStorage(configuration);

var app = builder.Build();
var dockerRunning = app.Configuration["DOCKER_RUNNING"];

if (app.Environment.IsDevelopment() || !string.IsNullOrEmpty(dockerRunning))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app
    .UseLoggingRequests()
    .UseExceptionHandling();

app.MapControllers();

app.Run();