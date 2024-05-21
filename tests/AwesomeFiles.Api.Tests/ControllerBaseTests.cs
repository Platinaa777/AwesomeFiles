using Microsoft.AspNetCore.Mvc;

namespace AwesomeFiles.Api.Tests;

public class ControllerBaseTests
{
    protected T GetControllerResultContent<T>(ActionResult<T> result)
    {
        return (T) ((ObjectResult) result.Result!).Value!;
    }
}