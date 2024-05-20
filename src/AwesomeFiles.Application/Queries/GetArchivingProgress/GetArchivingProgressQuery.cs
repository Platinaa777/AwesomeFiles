using MediatR;

namespace AwesomeFiles.Application.Queries.GetArchivingProgress;

public record GetArchivingProgressQuery(long ProcessId) : IRequest<bool>;
