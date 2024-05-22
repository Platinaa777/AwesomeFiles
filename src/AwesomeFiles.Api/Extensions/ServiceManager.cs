using System.Reflection;
using AwesomeFiles.Api.Middlewares;
using AwesomeFiles.Application.AssemblyInfo;
using AwesomeFiles.Application.Behaviors;
using AwesomeFiles.Application.Cache;
using AwesomeFiles.Application.Commands.StartArchiveProcess;
using AwesomeFiles.Application.Models;
using AwesomeFiles.Application.Queries.DownloadArchive;
using AwesomeFiles.Application.Services;
using AwesomeFiles.Domain.Models.ArchiveFileModel.Repos;
using AwesomeFiles.Domain.Models.WorkingProcessModel.Repos;
using AwesomeFiles.Infrastructure.Options;
using AwesomeFiles.Infrastructure.Repositories;
using AwesomeFiles.Infrastructure.Services;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;
using NpgsqlTypes;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL;

namespace AwesomeFiles.Api.Extensions;

public static class ServiceManager
{
    public static IServiceCollection AddApiLogging(this IServiceCollection services,
        IConfiguration configuration)
    {
        IDictionary<string, ColumnWriterBase> columnWriters = new Dictionary<string, ColumnWriterBase>
        {
            { "log", new RenderedMessageColumnWriter() },
            { "log_template", new MessageTemplateColumnWriter() },
            { "level", new LevelColumnWriter(true, NpgsqlDbType.Varchar) },
            { "request_time", new TimestampColumnWriter(NpgsqlDbType.TimestampTz) },
            { "props", new LogEventSerializedColumnWriter() }
        };
        
        services.AddLogging(b => b.AddSerilog(new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .WriteTo.Console()
            .WriteTo.PostgreSQL(configuration.GetConnectionString("Logs")!, "logs", columnWriters, needAutoCreateTable: true)
            .CreateLogger()));

        return services;
    }
   

    public static IServiceCollection AddApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddControllers();
        
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<StartArchiveProcessCommand>());

        services
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>))
            .AddTransient(typeof(IPipelineBehavior<DownloadArchiveQuery, CompletedArchive>), typeof(CacheArchiveBehavior));
            
        services.AddValidatorsFromAssembly(
            ApplicationAssembly.Assembly,
            includeInternalTypes: true);

        services.AddAutoMapper(config => 
            config.AddMaps(Assembly.GetEntryAssembly(), 
                ApplicationAssembly.Assembly));

        services
            .AddSingleton<IArchiveProcessRepository, ArchiveProcessRepository>()
            .AddSingleton<IFileRepository, LocalSystemFileRepository>();

        services
            .AddScoped<IArchiveService, LocalSystemArchiveService>();
        
        services.Configure<FileSystemStorageOptions>(
            configuration.GetSection("FileSystemStorageOptions"));
        services.AddSingleton<FileSystemStorageOptions>(
            sp => sp.GetRequiredService<IOptions<FileSystemStorageOptions>>().Value);
        
        return services;
    }

    public static WebApplicationBuilder ConfigureArchiveStorage(this WebApplicationBuilder builder,
        IConfiguration configuration)
    {
        string storageFolder = configuration["FileSystemStorageOptions:StorageFolder"] 
                               ?? throw new Exception("Не зарегистрирована папка чудесных файлов");
        string archiveFolder = configuration["FileSystemStorageOptions:ArchiveFolder"] 
                               ?? throw new Exception("Не зарегистрирована архив папка");
        
        // Проверка на хранение файлов в хранилище правда, из условия я так понимаю оно всегда должно быть создано уже 
        // с заранее заготовленными файлами, но оставлю как есть
        if (!Directory.Exists(storageFolder))
            Directory.CreateDirectory(storageFolder);
        
        // Создание хранилища архивов c нуля (как указано в задании)
        if (Directory.Exists(archiveFolder))
        {
            Directory.Delete(archiveFolder, recursive: true);
            Directory.CreateDirectory(archiveFolder);
        }

        return builder;
    }
    
    public static IServiceCollection AddCustomMiddlewares(this IServiceCollection services)
    {
        services
            .AddSingleton<ExceptionMiddleware>()
            .AddSingleton<RequestLoggingMiddleware>();

        return services;
    }
    
    public static IServiceCollection AddCaching(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Решил что буду использовать in memory кеш, использование редиса все-таки здесь не слишком оправдано
        services.AddMemoryCache();
        
        services.Configure<CacheOptions>(
            configuration.GetSection("CacheOptions"));
        services.AddSingleton<CacheOptions>(
            sp => sp.GetRequiredService<IOptions<CacheOptions>>().Value);
        
        return services;
    }
}