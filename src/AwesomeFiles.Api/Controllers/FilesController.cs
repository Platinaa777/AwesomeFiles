using AwesomeFiles.Application.Queries.GetStorageFiles;
using AwesomeFiles.HttpModels.Responses;
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
    public async Task<ActionResult<ApiResponse<List<FileInfoResponse>>>> GetAllFiles(
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetStorageFilesQuery(), cancellationToken);
        
        var files = result
            .Select(x => new FileInfoResponse(x.Name))
            .ToList();
        
        return Ok(ApiResponse<List<FileInfoResponse>>.ReturnSuccess(files));
    }
}