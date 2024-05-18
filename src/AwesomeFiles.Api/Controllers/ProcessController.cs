using AutoMapper;
using AwesomeFiles.Application.Commands.StartArchiveProcess;
using AwesomeFiles.HttpModels.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace AwesomeFiles.Api.Controllers;

[ApiController]
[Route("process")]
public class ProcessController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public ProcessController(
        IMediator mediator,
        IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    [HttpPost("start")]
    public async Task<ActionResult> StartArchivingFiles([FromBody] ListArchiveFiles fileNames, CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(_mapper.Map<StartArchiveProcessCommand>(fileNames), cancellationToken);
        
        return Ok(result);
    }

    [HttpGet("{processId:int}")]
    public async Task<ActionResult> CheckProcessStatus([FromRoute] int processId)
    {
        return Ok();
    }

    [HttpPost("{processId:int}")]
    public async Task<ActionResult> DownloadArchivedFiles([FromRoute] int processId)
    {
        return Ok();
    }
}