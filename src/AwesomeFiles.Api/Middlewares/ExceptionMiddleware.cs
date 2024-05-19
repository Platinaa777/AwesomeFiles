using System.Net;
using AwesomeFiles.Domain.Exceptions;
using Newtonsoft.Json;

namespace AwesomeFiles.Api.Middlewares;

public class ExceptionMiddleware : AbstractExceptionHandlerMiddleware
{
    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger) : base(logger)
    {
    }

    protected override (HttpStatusCode code, string message) GetSpecificResponse(Exception exception)
    {
        HttpStatusCode code;
        string errorMessage = exception.Message;
        switch (exception)
        {
            case ArchiveFileException:
                code = HttpStatusCode.BadRequest;
                break;
            case ArchiveNotFoundException:
                code = HttpStatusCode.NotFound;
                break;
            default:
                code = HttpStatusCode.InternalServerError;
                errorMessage = "Some server error. Please try later";
                break;
        }
        return (code, JsonConvert.SerializeObject(new {errorMessage}));
    }
}

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ExceptionMiddleware>();
    }
}