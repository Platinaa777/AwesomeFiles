using System.Collections.Concurrent;
using AwesomeFiles.Domain.Models.ArchiveFileModel;
using AwesomeFiles.Domain.Models.ArchiveFileModel.Repos;

namespace AwesomeFiles.Infrastructure.Repositories;

public class AchiveFileRepository : IArchiveFileRepository
{
    private readonly ConcurrentDictionary<string, ArchiveFile> storage = new();
    
    public Task<ArchiveFile> GetAllFiles()
    {
        throw new NotImplementedException();
    }

    public Task<ArchiveFile?> GetFileByPath()
    {
        throw new NotImplementedException();
    }

    public Task<bool> Add()
    {
        throw new NotImplementedException();
    }
}