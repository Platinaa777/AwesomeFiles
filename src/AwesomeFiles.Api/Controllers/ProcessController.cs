using AutoMapper;
using AwesomeFiles.Api.Enums;
using AwesomeFiles.Application.Commands.StartArchiveProcess;
using AwesomeFiles.Application.Query.DownloadArchive;
using AwesomeFiles.Application.Query.GetArchivingProgress;
using AwesomeFiles.HttpModels.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static AwesomeFiles.Api.Enums.ProcessStatus;

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
        var result = await _mediator.Send(new GetArchivingProgressQuery(processId));
        
        return Ok(result ? Completed.ToString() : Pending.ToString());
    }

    [HttpGet("download/{processId:int}")]
    public async Task<ActionResult> DownloadArchivedFiles([FromRoute] int processId)
    {
        var result = await _mediator.Send(new DownloadArchiveQuery(processId));

        if (!result.IsReady)
            return BadRequest(Pending.ToString());

        return File(result.Stream, "application/zip", $"archieve-{processId}");
    }
}