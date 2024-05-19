using System.IO.Compression;
using AwesomeFiles.Application.Services;
using AwesomeFiles.Application.Services.Models;
using AwesomeFiles.Domain.Errors;
using AwesomeFiles.Domain.Exceptions;
using AwesomeFiles.Domain.ResultAbstractions;
using AwesomeFiles.Infrastructure.Constants;
using ZipFile = System.IO.Compression.ZipFile;

namespace AwesomeFiles.Infrastructure.Services;

public class LocalSystemArchiveService : IArchiveService
{
    private static long _counter;
    
    public Result CheckAllFilesExists(List<string> files)
    {
        List<Error> errors = new();
        try
        {
            foreach (var file in files)
            {
                if (!File.Exists(Path.Combine(FileSystemStorageConstants.StorageFolder, file)))
                    errors.Add(FileError.AddFileNotExistsError(file));
            }
        }
        catch { /* ignored */ }

        return errors.Any() ? Result.Failure(errors.ToArray()) : Result.Success();
    }
    
    public ArchiveTask LaunchArchiving(List<string> existingFiles)
    {
        var processId = Interlocked.Add(ref _counter, 1);

        // Запуск асинхронной архивации файлов
        var task = Task.Run(() =>
        {
            var curArchiveFolder = Path.Combine(FileSystemStorageConstants.ArchiveFolder, $"archive-{processId}");
            var zipFile = curArchiveFolder + ".zip";
            // Запуск архивации файлов
            CreateZipFromFiles(zipFile, existingFiles.ToArray());
        });

        return new ArchiveTask { ProcessId = processId, WorkItem = task };
    }

    public MemoryStream DownloadArchive(long processId)
    {
        var archivePath = Path.Combine(FileSystemStorageConstants.ArchiveFolder, $"archive-{processId}.zip");
        if (!File.Exists(archivePath))
            throw new ArchiveNotFoundException($"Архив с названием {processId} не был найден"); 

        var memory = new MemoryStream();
        using (var stream = new FileStream(archivePath, FileMode.Open, FileAccess.Read))
        {
            stream.CopyTo(memory);
        }
        memory.Position = 0;
        
        return memory;
    }
    
    private void CreateZipFromFiles(string zipFilePath, string[] filePaths)
    {
        try
        {
            using var zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Create);
            foreach (var file in filePaths)
                zip.CreateEntryFromFile(Path.Combine(FileSystemStorageConstants.StorageFolder, file), file);
        }
        catch (Exception)
        {
            // случай когда кто-то удалил файл из storage, но при этом прошла проверка через 
            // метод CheckAllFilesExists
            throw new ArchiveFileException("Файл из списка был удален");
        }
    }
}