using System.CommandLine;
using af.Commands;
using af.Services;
using Microsoft.Extensions.DependencyInjection;

namespace af.Extensions;

public static class DependencyInjection
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<Command, AutoCheckingCommand>();
        services.AddTransient<Command, CreateArchiveCommand>();
        services.AddTransient<Command, DownloadCommand>();
        services.AddTransient<Command, ExitCommand>();
        services.AddTransient<Command, ListCommand>();
        services.AddTransient<Command, StatusCommand>();
        services.AddTransient<IFileService, FileService>();

        services.AddHttpClient();
    }
}