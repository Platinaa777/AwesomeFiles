using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using af.Services;

namespace af.Commands;

public class AutoCheckingCommand : Command
{
    private readonly IFileService _fileService;
    private const string NAME = "auto-create-archive";
    private const string DESC = "Режим который самостоятельно запрашивает у backend создание архива, опрашивание о его готовности и скачивание (опрос происходит каждые 200ms)";
    
    public AutoCheckingCommand(IHttpClientFactory factory, IFileService fileService) : base(NAME, DESC)
    {
        _fileService = fileService;
        AddArgument(new Argument<string>("path", "Путь куда будет скачан архив"));
        AddArgument(new Argument<string[]>("files", "Файлы которые должны быть архивированы"));
        
        Handler = CommandHandler.Create<string, string[]>(async (path,files) =>
        {
            var processId = await new CreateArchiveCommand(factory)
                .InvokeAsync(files);

            // Не смогли создать процесс на архивацию
            if (processId == -1)
                return -1;

            var isReady = false;
            do
            {
                var statusCommandExitCode = await new StatusCommand(factory)
                    .InvokeAsync(new []{ processId.ToString() });
                
                if (statusCommandExitCode == -1)
                    return -1;
                if (statusCommandExitCode == processId)
                    isReady = true;
                
                await Task.Delay(200);
            } while (!isReady);
        
            var exitCode = await new DownloadCommand(factory, fileService)
                .InvokeAsync(new []{ processId.ToString(), path});

            return exitCode;
        });
    }
}