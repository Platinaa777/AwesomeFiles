using AwesomeFiles.Application.Models;
using MediatR;

namespace AwesomeFiles.Application.Query.DownloadArchive;

public record DownloadArchiveQuery(long ProcessId) : IRequest<CompletedArchive>;
