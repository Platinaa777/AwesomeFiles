using System.Collections.Concurrent;
using AwesomeFiles.Domain.Models.WorkingProcessModel.Repos;

namespace AwesomeFiles.Infrastructure.Repositories;

public class ArchiveProcessRepository : IArchiveProcessRepository
{
    private readonly ConcurrentDictionary<long, Task> _storage = new();
    
    public void AddWithId(long id, Task archivingProcess)
    {
        _storage[id] = archivingProcess;
    }

    public Task? GetById(long id)
    {
        return _storage.GetValueOrDefault(id);
    }
}