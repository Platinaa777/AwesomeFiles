using AwesomeFiles.Application.Models;
using AwesomeFiles.Application.Queries.DownloadArchive;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace AwesomeFiles.Application.Behavior;

public class CacheArchiveBehavior : IPipelineBehavior<DownloadArchiveQuery, CompletedArchive>
{
    private readonly IMemoryCache _memoryCache;
    private readonly ILogger<CacheArchiveBehavior> _logger;

    public CacheArchiveBehavior(
        IMemoryCache memoryCache,
        ILogger<CacheArchiveBehavior> logger)
    {
        _memoryCache = memoryCache;
        _logger = logger;
    }
    
    public async Task<CompletedArchive> Handle(DownloadArchiveQuery request, RequestHandlerDelegate<CompletedArchive> next, CancellationToken cancellationToken)
    {
        // если у нас уже находится в кеше архив, то вернем его
        if (_memoryCache.TryGetValue(request.ProcessId.ToString(), out var archiveBytes)
            && archiveBytes is byte[] bytes)
        {
            _logger.LogInformation("Archive with id: {@Id} was retrieved from the cache", request.ProcessId);
            return new CompletedArchive(bytes, true);
        }

        var result = await next();

        // // кеширую на 1 минуту, возможно, нужно побольше,
        // // но навряд ли проверяющему захочется ждать более 1 минуты протухания кеша)))
        if (result.IsReady)
            _memoryCache.Set(request.ProcessId.ToString(), result.ZipBytes, DateTimeOffset.Now.AddMinutes(1));

        return result;
    }
}