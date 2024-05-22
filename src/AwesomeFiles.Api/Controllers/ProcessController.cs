using AutoMapper;
using AwesomeFiles.Api.Enums;
using AwesomeFiles.Application.Commands.StartArchiveProcess;
using AwesomeFiles.Application.Queries.DownloadArchive;
using AwesomeFiles.Application.Queries.GetArchivingProgress;
using AwesomeFiles.HttpModels.Requests;
using AwesomeFiles.HttpModels.Responses;
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
    public async Task<ActionResult<ApiResponse<ProcessIdResponse>>> StartArchivingFiles(
        [FromBody] ListArchiveFiles fileNames,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(_mapper.Map<StartArchiveProcessCommand>(fileNames),
            cancellationToken);

        if (result.IsFailure)
        {
            var errorMessages = result.Errors
                .Select(x => x.Message)
                .ToList();
            
            var response = ApiResponse<ProcessIdResponse>
                .ReturnFailure(errorMessages);
            
            return BadRequest(response);
        }
        
        var responseValue = new ProcessIdResponse(result.Value.Id);
        
        return Ok(ApiResponse<ProcessIdResponse>.ReturnSuccess(responseValue));
    }

    [HttpGet("{processId:int}")]
    public async Task<ActionResult<ApiResponse<ArchivingStatus>>> CheckProcessStatus(
        [FromRoute] int processId)
    {
        var result = await _mediator.Send(new GetArchivingProgressQuery(processId));

        var convertedResponse = ProcessUtils.ConvertToApiResponse(result);
        return Ok(ApiResponse<ArchivingStatus>.ReturnSuccess(convertedResponse));
    }

    [HttpGet("download/{processId:int}")]
    public async Task<ActionResult> DownloadArchivedFiles(
        [FromRoute] int processId)
    {
        var result = await _mediator.Send(new DownloadArchiveQuery(processId));

        if (!result.IsReady)
            return BadRequest(ProcessUtils.ConvertToApiResponse(false));
        
        return File(result.ZipBytes, "application/zip", $"archive-{processId}");
    }
}