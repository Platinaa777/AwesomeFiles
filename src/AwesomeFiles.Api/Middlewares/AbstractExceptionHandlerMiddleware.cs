using System.Net;

namespace AwesomeFiles.Api.Middlewares;

public abstract class AbstractExceptionHandlerMiddleware : IMiddleware
{
    private readonly ILogger<AbstractExceptionHandlerMiddleware> _logger;

    protected AbstractExceptionHandlerMiddleware(ILogger<AbstractExceptionHandlerMiddleware> logger)
    {
        _logger = logger;
    }

    protected abstract (HttpStatusCode code, string message) GetSpecificResponse(Exception exception);

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            _logger.LogError("Error while executing request: {@RequestPath} with error message: {@ErrorMessage}",
                context.Request.Path.Value,
                e.Message);
            var response = context.Response;
            response.ContentType = "application/json";
            
            var (status, message) = GetSpecificResponse(e);
            response.StatusCode = (int)status;
            await response.WriteAsync(message);
        }
    }
}