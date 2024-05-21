using AwesomeFiles.Api.Controllers;
using AwesomeFiles.Application.Queries.GetStorageFiles;
using Microsoft.AspNetCore.Mvc;


namespace AwesomeFiles.Api.Tests;

public class FilesControllerShould : ControllerBaseTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly FilesController _controller;

    public FilesControllerShould()
    {
        _controller = new(_mediatorMock.Object);
    }

    [Fact]
    public async Task ReturnEmptyList_WhenFilesNotExist()
    {
        _mediatorMock.Setup(x => x.Send(It.IsAny<GetStorageFilesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GetStorageFileModel>());

        var result = await _controller.GetAllFiles(CancellationToken.None);

        result.Result.Should().BeOfType<OkObjectResult>();
        var apiResponse = GetControllerResultContent(result);
        apiResponse.Errors.Should().BeNull();
        apiResponse.Body.Should().BeEmpty();
    }
    
    [Fact]
    public async Task ReturnFilesList_WhenFilesExist()
    {
        _mediatorMock.Setup(x => x.Send(It.IsAny<GetStorageFilesQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<GetStorageFileModel> { new("file1"), new("file100")});

        var result = await _controller.GetAllFiles(CancellationToken.None);
        
        result.Result.Should().BeOfType<OkObjectResult>();
        var apiResponse = GetControllerResultContent(result);
        
        apiResponse.Success.Should().BeTrue();
        apiResponse.Body!.DistinctBy(x => x.Name).Count().Should().Be(2);
    }
}