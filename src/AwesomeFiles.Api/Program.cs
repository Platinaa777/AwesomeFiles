using AwesomeFiles.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Logging.ClearProviders();

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

builder.Services
    .AddApiLogging()
    .AddApplicationServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();