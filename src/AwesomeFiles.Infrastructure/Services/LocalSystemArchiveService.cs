using System.IO.Compression;
using AwesomeFiles.Application.Services;
using AwesomeFiles.Application.Services.Models;
using AwesomeFiles.Domain.Errors;
using AwesomeFiles.Domain.Exceptions;
using AwesomeFiles.Domain.ResultAbstractions;
using AwesomeFiles.Infrastructure.Options;
using ZipFile = System.IO.Compression.ZipFile;

namespace AwesomeFiles.Infrastructure.Services;

public class LocalSystemArchiveService : IArchiveService
{
    private readonly FileSystemStorageOptions _fileSystemStorageOptions;
    private static long _counter;

    public LocalSystemArchiveService(
        FileSystemStorageOptions fileSystemStorageOptions )
    {
        _fileSystemStorageOptions = fileSystemStorageOptions;
    }
    
    public Result CheckAllFilesExist(List<string> files)
    {
        List<Error> errors = new();
        try
        {
            foreach (var file in files)
            {
                if (!File.Exists(Path.Combine(_fileSystemStorageOptions.StorageFolder, file)))
                    errors.Add(FileError.FileNotExistsError(file));
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
            var curArchiveFolder = Path.Combine(_fileSystemStorageOptions.ArchiveFolder, $"archive-{processId}");
            var zipFile = curArchiveFolder + ".zip";
            // Запуск архивации файлов
            CreateZipFromFiles(zipFile, existingFiles.ToArray());
        });

        return new ArchiveTask { ProcessId = processId, WorkItem = task };
    }

    public async Task<byte[]> DownloadArchiveAsync(long processId)
    {
        var archivePath = Path.Combine(_fileSystemStorageOptions.ArchiveFolder, $"archive-{processId}.zip");
        if (!File.Exists(archivePath))
            throw new ArchiveNotFoundException($"Архив с названием {processId} не был найден");
        
        var bytes = await File.ReadAllBytesAsync(archivePath);
        return bytes;
    }
    
    private void CreateZipFromFiles(string zipFilePath, string[] files)
    {
        try
        {
            using var zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Create);
            foreach (var file in files)
                zip.CreateEntryFromFile(
                    sourceFileName: Path.Combine(_fileSystemStorageOptions.StorageFolder, file),
                    entryName: file);
        }
        catch (Exception)
        {
            // случай когда кто-то удалил файл из storage, но при этом прошла проверка через 
            // метод CheckAllFilesExists
            throw new ArchiveFileException("Файл из списка был удален");
        }
    }
}