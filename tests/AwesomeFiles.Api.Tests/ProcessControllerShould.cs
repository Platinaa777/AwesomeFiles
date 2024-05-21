using AutoMapper;
using AwesomeFiles.Api.Controllers;
using AwesomeFiles.Api.Enums;
using AwesomeFiles.Application.Commands.StartArchiveProcess;
using AwesomeFiles.Application.Models;
using AwesomeFiles.Application.Queries.DownloadArchive;
using AwesomeFiles.Application.Queries.GetArchivingProgress;
using AwesomeFiles.Domain.Errors;
using AwesomeFiles.Domain.ResultAbstractions;
using AwesomeFiles.HttpModels.Requests;
using Microsoft.AspNetCore.Mvc;

namespace AwesomeFiles.Api.Tests;

public class ProcessControllerShould : ControllerBaseTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly Mock<IMapper> _mapperMock = new();
    private readonly ProcessController _controller;

    public ProcessControllerShould()
    {
        _controller = new(_mediatorMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task ReturnBadRequest_WhenProcessCantBeStart()
    {
        var files = new List<string> { "file-1" };
        
        _mediatorMock.Setup(x => x.Send(It.IsAny<StartArchiveProcessCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Failure<ProcessId>(FileError.FileNotExistsError("file-1")));
        _mapperMock.Setup(x => x.Map<StartArchiveProcessCommand>(It.IsAny<ListArchiveFiles>()))
            .Returns(new StartArchiveProcessCommand(files));

        var response = await _controller.StartArchivingFiles(new ListArchiveFiles { FileNames = files });

        response.Result.Should().BeOfType<BadRequestObjectResult>();
        
        var apiResponse = GetControllerResultContent(response);
        apiResponse.Body.Should().BeNull();
        apiResponse.Errors!.Count.Should().Be(1);
    }
    
    [Fact]
    public async Task ReturnOkWithProcessId_WhenProcessWasSuccessfullyStarted()
    {
        var files = new List<string> { "file-1" };
        
        _mediatorMock.Setup(x => x.Send(It.IsAny<StartArchiveProcessCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(new ProcessId(1)));
        _mapperMock.Setup(x => x.Map<StartArchiveProcessCommand>(It.IsAny<ListArchiveFiles>()))
            .Returns(new StartArchiveProcessCommand(files));

        var response = await _controller.StartArchivingFiles(new ListArchiveFiles { FileNames = files });

        response.Result.Should().BeOfType<OkObjectResult>();
        
        var apiResponse = GetControllerResultContent(response);
        apiResponse.Errors.Should().BeNull();
        apiResponse.Body!.Id.Should().Be(1);
    }

    [Fact]
    public async Task ReturnCompleteStatusOfProcess_WhenArchivingProcessIsReady()
    {
        _mediatorMock.Setup(x => x.Send(It.IsAny<GetArchivingProgressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var response = await _controller.CheckProcessStatus(1);

        response.Result.Should().BeOfType<OkObjectResult>();
        
        var apiResponse = GetControllerResultContent(response);
        apiResponse.Errors.Should().BeNull();
        apiResponse.Body!.Status.Should().Be(ProcessStatus.Completed.ToString());
    }
    
    [Fact]
    public async Task ReturnPendingStatusOfProcess_WhenArchivingProcessInProgress()
    {
        _mediatorMock.Setup(x => x.Send(It.IsAny<GetArchivingProgressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var response = await _controller.CheckProcessStatus(1);

        response.Result.Should().BeOfType<OkObjectResult>();
        
        var apiResponse = GetControllerResultContent(response);
        apiResponse.Errors.Should().BeNull();
        apiResponse.Body!.Status.Should().Be(ProcessStatus.Pending.ToString());
    }
    
    [Fact]
    public async Task ReturnPendingStatus_WhenTryToDownloadArchive_WhichNotReady()
    {
        var archive = new CompletedArchive(false);
        
        _mediatorMock.Setup(x => x.Send(It.IsAny<DownloadArchiveQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(archive);

        var response = await _controller.DownloadArchivedFiles(1);

        response.Should().BeOfType<BadRequestObjectResult>();
    }
    
    [Fact]
    public async Task ReturnFileContent_WhenArchiveIsReadyToDownload()
    {
        var archive = new CompletedArchive(new byte[]{1,2,3}, true);
        
        _mediatorMock.Setup(x => x.Send(It.IsAny<DownloadArchiveQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(archive);

        var response = await _controller.DownloadArchivedFiles(1);

        response.Should().BeOfType<FileContentResult>();
    }
}