namespace AwesomeFiles.Api.Middlewares;

public class RequestLoggingMiddleware : IMiddleware
{
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger)
    {
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var traceId = context.TraceIdentifier;
        
        _logger.LogInformation("Request with trace id: {@TraceId} to endpoint: {@RequestPath}",
            traceId, context.Request.Path.Value);

        await next(context);
        
        _logger.LogInformation("Request with trace id: {@TraceId} to endpoint: {@RequestPath} with status code: {@StatusCode}",
            traceId, context.Request.Path.Value, context.Response.StatusCode);
    }
}

public static class RequestMiddlewareExtensions
{
    public static IApplicationBuilder UseLoggingRequests(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestLoggingMiddleware>();
    }
}