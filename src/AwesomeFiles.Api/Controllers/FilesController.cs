using Microsoft.AspNetCore.Mvc;

namespace AwesomeFiles.Api.Controllers;

[ApiController]
[Route("files")]
public class FilesController : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult> GetAllFiles()
    {
        return Ok();
    }
}