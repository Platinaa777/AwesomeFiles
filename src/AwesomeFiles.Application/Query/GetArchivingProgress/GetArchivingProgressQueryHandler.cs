using AwesomeFiles.Domain.Exceptions;
using AwesomeFiles.Domain.Models.WorkingProcessModel.Repos;
using MediatR;

namespace AwesomeFiles.Application.Query.GetArchivingProgress;

public class GetArchivingProgressQueryHandler
    : IRequestHandler<GetArchivingProgressQuery, bool>
{
    private readonly IArchiveProcessRepository _archiveProcessRepository;

    public GetArchivingProgressQueryHandler(
        IArchiveProcessRepository archiveProcessRepository)
    {
        _archiveProcessRepository = archiveProcessRepository;
    }
    
    public Task<bool> Handle(GetArchivingProgressQuery request, CancellationToken cancellationToken)
    {
        var result = _archiveProcessRepository.GetById(request.ProcessId);

        if (result is null)
            throw new ArchiveNotFoundException($"Архив с id: {request.ProcessId} не был найден");

        return result.Status switch
        {
            TaskStatus.RanToCompletion => Task.FromResult(true),
            TaskStatus.Faulted => throw result.Exception!,
            _ => Task.FromResult(false)
        };
    }
}