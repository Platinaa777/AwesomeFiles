using AwesomeFiles.Domain.Models.ArchiveFileModel;
using AwesomeFiles.Domain.Models.ArchiveFileModel.Repos;
using AwesomeFiles.Infrastructure.Constants;

namespace AwesomeFiles.Infrastructure.Repositories;

public class LocalSystemArchiveFileRepository : IArchiveFileRepository
{
    public List<ArchiveFile> GetAllFiles()
        => Directory.GetFiles(FileSystemStorageConstants.StorageFolder)
            .Select(fileName => new ArchiveFile(Path.GetFileName(fileName))).ToList();
}