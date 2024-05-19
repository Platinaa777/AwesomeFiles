using AwesomeFiles.Application.Query.GetStorageFiles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AwesomeFiles.Api.Controllers;

[ApiController]
[Route("files")]
public class FilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public FilesController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet]
    public async Task<ActionResult> GetAllFiles(CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetStorageFilesQuery(), cancellationToken);
        return Ok(result);
    }
}