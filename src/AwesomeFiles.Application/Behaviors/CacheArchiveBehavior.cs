using AwesomeFiles.Application.Cache;
using AwesomeFiles.Application.Models;
using AwesomeFiles.Application.Queries.DownloadArchive;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace AwesomeFiles.Application.Behaviors;

public class CacheArchiveBehavior : IPipelineBehavior<DownloadArchiveQuery, CompletedArchive>
{
    private readonly IMemoryCache _memoryCache;
    private readonly CacheOptions _cacheOptions;
    private readonly ILogger<CacheArchiveBehavior> _logger;

    public CacheArchiveBehavior(
        IMemoryCache memoryCache,
        CacheOptions cacheOptions,
        ILogger<CacheArchiveBehavior> logger)
    {
        _memoryCache = memoryCache;
        _cacheOptions = cacheOptions;
        _logger = logger;
    }
    
    public async Task<CompletedArchive> Handle(DownloadArchiveQuery request, RequestHandlerDelegate<CompletedArchive> next, CancellationToken cancellationToken)
    {
        // если у нас уже находится в кеше архив, то вернем его
        if (_memoryCache.TryGetValue(request.ProcessId.ToString(), out var archiveBytes)
            && archiveBytes is byte[] bytes)
        {
            _logger.LogInformation("Archive with id: {@Id} was retrieved from the cache",
                request.ProcessId);
            
            return new CompletedArchive(bytes, true);
        }

        var result = await next();

        if (result.IsReady)
            _memoryCache.Set(
                key: request.ProcessId.ToString(),
                value: result.ZipBytes,
                absoluteExpiration: DateTimeOffset.Now.AddMinutes(_cacheOptions.ExpiryTimeInMinutes));

        return result;
    }
}