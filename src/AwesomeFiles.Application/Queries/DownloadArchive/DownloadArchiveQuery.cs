using AwesomeFiles.Application.Models;
using MediatR;

namespace AwesomeFiles.Application.Queries.DownloadArchive;

public record DownloadArchiveQuery(long ProcessId) : IRequest<CompletedArchive>;
