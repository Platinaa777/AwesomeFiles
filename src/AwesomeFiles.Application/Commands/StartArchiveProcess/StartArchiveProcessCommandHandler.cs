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
    private readonly IWorkingProcessRepository _processRepository;

    public StartArchiveProcessCommandHandler(
        IArchiveService archiveService,
        IWorkingProcessRepository processRepository)
    {
        _archiveService = archiveService;
        _processRepository = processRepository;
    }
    
    public async Task<Result<ProcessId>> Handle(StartArchiveProcessCommand request, CancellationToken cancellationToken)
    {
        // Проверяем что файлы все существуют в системе
        var result = _archiveService.CheckFiles(request.Files);
        if (result.IsFailure)
            return Result.Failure<ProcessId>(result.Errors.ToArray());

        var archiveTask = _archiveService.LaunchArchiving(request.Files);


        return Result.Success(new ProcessId("das"));
    }
}