using System.Reflection;
using AwesomeFiles.Application.AssemblyInfo;
using AwesomeFiles.Application.Behavior;
using AwesomeFiles.Application.Commands.StartArchiveProcess;
using AwesomeFiles.Application.Services;
using AwesomeFiles.Domain.Models.ArchiveFileModel.Repos;
using AwesomeFiles.Domain.Models.WorkingProcessModel.Repos;
using AwesomeFiles.Infrastructure.Repositories;
using AwesomeFiles.Infrastructure.Services;
using FluentValidation;
using MediatR;
using Serilog;
using Serilog.Events;

namespace AwesomeFiles.Api.Extensions;

public static class ServiceManager
{
    public static IServiceCollection AddApiLogging(this IServiceCollection services) => 
        services.AddLogging(b => b.AddSerilog(new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .WriteTo.Logger(logCfg => logCfg.WriteTo.Console())
            .CreateLogger()));

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddControllers();
        
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<StartArchiveProcessCommand>());

        services
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
            
        services.AddValidatorsFromAssembly(
            ApplicationAssembly.Assembly,
            includeInternalTypes: true);

        services.AddAutoMapper(config => 
            config.AddMaps(Assembly.GetEntryAssembly(), 
                ApplicationAssembly.Assembly));

        services
            .AddSingleton<IWorkingProcessRepository, WorkingProcessRepository>()
            .AddSingleton<IArchiveFileRepository, AchiveFileRepository>();

        services
            .AddScoped<IArchiveService, LocalSystemArchiveService>();
        
        return services;
    } 
}