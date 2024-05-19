using AwesomeFiles.Application.Models;
using AwesomeFiles.Application.Services;
using AwesomeFiles.Domain.Exceptions;
using AwesomeFiles.Domain.Models.WorkingProcessModel.Repos;
using MediatR;

namespace AwesomeFiles.Application.Query.DownloadArchive;

public class DownloadArchiveQueryHandler
    : IRequestHandler<DownloadArchiveQuery, CompletedArchive>
{
    private readonly IArchiveProcessRepository _archiveProcessRepository;
    private readonly IArchiveService _archiveService;

    public DownloadArchiveQueryHandler(
        IArchiveProcessRepository archiveProcessRepository,
        IArchiveService archiveService)
    {
        _archiveProcessRepository = archiveProcessRepository;
        _archiveService = archiveService;
    }
    
    public Task<CompletedArchive> Handle(DownloadArchiveQuery request, CancellationToken cancellationToken)
    {
        var result = _archiveProcessRepository.GetById(request.ProcessId);
        if (result is null)
            throw new ArchiveNotFoundException($"Архив с id: {request.ProcessId} не был найден");
        
        if (result.Status != TaskStatus.RanToCompletion)
            return Task.FromResult(new CompletedArchive(false));

        var zipStream = _archiveService.DownloadArchive(request.ProcessId);

        return Task.FromResult(new CompletedArchive(zipStream, true));
    }
}