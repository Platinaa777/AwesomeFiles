using MediatR;

namespace AwesomeFiles.Application.Query.GetArchivingProgress;

public record GetArchivingProgressQuery(long ProcessId) : IRequest<bool>;
