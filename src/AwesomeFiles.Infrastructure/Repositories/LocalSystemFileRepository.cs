using AwesomeFiles.Domain.Models.ArchiveFileModel;
using AwesomeFiles.Domain.Models.ArchiveFileModel.Repos;
using AwesomeFiles.Infrastructure.Options;

namespace AwesomeFiles.Infrastructure.Repositories;

public class LocalSystemFileRepository : IFileRepository
{
    private readonly FileSystemStorageOptions _fileSystemStorageOptions;

    public LocalSystemFileRepository(FileSystemStorageOptions fileSystemStorageOptions)
    {
        _fileSystemStorageOptions = fileSystemStorageOptions;
    }
    
    public List<AwesomeFile> GetAllFiles() =>
        Directory
            .GetFiles(_fileSystemStorageOptions.StorageFolder)
            .Select(fileName => new AwesomeFile(Path.GetFileName(fileName)))
            .ToList();
}