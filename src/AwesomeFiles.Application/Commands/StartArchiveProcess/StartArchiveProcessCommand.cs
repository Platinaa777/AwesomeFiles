using AwesomeFiles.Application.Models;
using AwesomeFiles.Domain.ResultAbstractions;
using MediatR;

namespace AwesomeFiles.Application.Commands.StartArchiveProcess;

public record StartArchiveProcessCommand(List<string> Files)
    : IRequest<Result<ProcessId>>;