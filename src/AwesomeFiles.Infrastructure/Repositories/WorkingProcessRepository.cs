using System.Collections.Concurrent;
using AwesomeFiles.Domain.Models.WorkingProcessModel;
using AwesomeFiles.Domain.Models.WorkingProcessModel.Repos;

namespace AwesomeFiles.Infrastructure.Repositories;

public class WorkingProcessRepository : IWorkingProcessRepository
{
    private readonly ConcurrentDictionary<string, Task<WorkingProcess>> storage = new();
    
    public Task Add()
    {
        throw new NotImplementedException();
    }

    public Task<WorkingProcess?> GetById()
    {
        throw new NotImplementedException();
    }
}