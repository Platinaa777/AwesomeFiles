using AwesomeFiles.Application.Models;
using AwesomeFiles.Application.Services;
using AwesomeFiles.Domain.Models.WorkingProcessModel.Repos;
using AwesomeFiles.Domain.ResultAbstractions;
using MediatR;

namespace AwesomeFiles.Application.Commands.StartArchiveProcess;

public class StartArchiveProcessCommandHandler
    : IRequestHandler<StartArchiveProcessCommand, Result<ProcessId>>
{
    private readonly IArchiveService _archiveService;
    private readonly IArchiveProcessRepository _processRepository;

    public StartArchiveProcessCommandHandler(
        IArchiveService archiveService,
        IArchiveProcessRepository processRepository)
    {
        _archiveService = archiveService;
        _processRepository = processRepository;
    }
    
    public Task<Result<ProcessId>> Handle(StartArchiveProcessCommand request, CancellationToken cancellationToken)
    {
        // Проверяем что файлы все существуют в системе
        var result = _archiveService.CheckAllFilesExist(request.Files);
        if (result.IsFailure)
            return Task.FromResult(Result.Failure<ProcessId>(result.Errors.ToArray()));

        var archiveTask = _archiveService.LaunchArchiving(request.Files);

        _processRepository.AddWithId(archiveTask.ProcessId, archiveTask.WorkItem);

        return Task.FromResult(Result.Success(new ProcessId(archiveTask.ProcessId)));
    }
}