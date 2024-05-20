using AwesomeFiles.Domain.Models.ArchiveFileModel;
using AwesomeFiles.Domain.Models.ArchiveFileModel.Repos;
using AwesomeFiles.Infrastructure.Constants;

namespace AwesomeFiles.Infrastructure.Repositories;

public class LocalSystemFileRepository : IFileRepository
{
    public List<AwesomeFile> GetAllFiles()
        => Directory.GetFiles(FileSystemStorageConstants.StorageFolder)
            .Select(fileName => new AwesomeFile(Path.GetFileName(fileName))).ToList();
}