using System.Diagnostics;
using AwesomeFiles.Application.Services;
using AwesomeFiles.Domain.Errors;
using AwesomeFiles.Domain.Models.WorkingProcessModel;
using AwesomeFiles.Domain.ResultAbstractions;

namespace AwesomeFiles.Infrastructure.Services;

public class LocalSystemArchiveService : IArchiveService
{
    public Result CheckFiles(List<string> files)
    {
        List<Error> errors = new();
        try
        {
            foreach (var file in files)
            {
                if (!File.Exists(file))
                    errors.Add(FileError.AddFileNotExistsError(file));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        return errors.Any() ? Result.Failure(errors.ToArray()) : Result.Success();
    }
    public WorkingProcess LaunchArchiving(List<string> files)
    {
        var operationId = Guid.NewGuid();
        // args[0]: Путь где будем архивировать (хранилище)
        // args[1]: Название самого архива
        
        // Раз в задании сказано, что нужно возвращать Id процесса. Я спроектировал
        // так, что каждый запущенный нами процесс создает новый уникальный архив
        // для этого будет нужен operationId:Guid
        // Id - for client side
        // OperationId - for my backend
        List<string> args = new() { "../../storage", $"archive-{operationId}" };
        foreach (var file in files)
            args.Add(file);
        
        var process = Process.Start("./archive-script.sh", args);
        
        return new WorkingProcess(process.Id, operationId, process);
    }

    public dynamic DownloadArchive(WorkingProcess process)
    {
        throw new NotImplementedException();
    }
}