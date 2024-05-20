using AwesomeFiles.Application.Commands.StartArchiveProcess;
using AwesomeFiles.Application.Services;
using AwesomeFiles.Application.Services.Models;
using AwesomeFiles.Domain.Errors;
using AwesomeFiles.Domain.Models.WorkingProcessModel.Repos;
using AwesomeFiles.Domain.ResultAbstractions;
using FluentAssertions;

namespace AwesomeFiles.Application.Tests.Commands.StartArchiveProcess;

public class StartArchiveProcessCommandShould
{
    private readonly StartArchiveProcessCommand _command;
    private readonly Mock<IArchiveService> _archiveServiceMock = new();
    private readonly Mock<IArchiveProcessRepository> _processRepositoryMock = new();
    private readonly StartArchiveProcessCommandHandler _handler;
    private readonly List<string> _requestFiles = new() { "file1", "file2" };

    public StartArchiveProcessCommandShould()
    {
        _handler = new(_archiveServiceMock.Object, _processRepositoryMock.Object);
        _command = new StartArchiveProcessCommand(_requestFiles);
    }

    [Fact]
    public async Task ReturnOneFileNotFoundError_WhenAnyFileNotExist()
    {
        string notExistFile = "file2";
        _archiveServiceMock.Setup(x => x.CheckAllFilesExists(_requestFiles))
            .Returns(Result.Failure(FileError.AddFileNotExistsError(notExistFile)));

        var result = await _handler.Handle(_command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Errors.Count.Should().Be(1);
    }
    
    [Fact]
    public async Task ReturnManyFileNotFoundErrors_When2OrMoreFilesNotExists()
    {
        string notExistFile1 = "file1";
        string notExistsFile2 = "file2";
        _archiveServiceMock.Setup(x => x.CheckAllFilesExists(_requestFiles))
            .Returns(Result.Failure(FileError.AddFileNotExistsError(notExistFile1), FileError.AddFileNotExistsError(notExistsFile2)));

        var result = await _handler.Handle(_command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Errors.Distinct().Count().Should().Be(2);
        result.Errors.Should().AllBeOfType<FileError>();
    }

    [Fact]
    public async Task ReturnSuccess_WhenAllFilesInStorage()
    {
        _archiveServiceMock.Setup(x => x.CheckAllFilesExists(_requestFiles))
            .Returns(Result.Success);
        _archiveServiceMock.Setup(x => x.LaunchArchiving(_requestFiles))
            .Returns(new ArchiveTask { ProcessId = 1, WorkItem = Task.CompletedTask });

        var result = await _handler.Handle(_command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(1);
    }
    
    [Fact]
    public async Task Handle_AddTaskToStorage_WhenSuccess()
    {
        var workItem = Task.CompletedTask;
        var id = 1;
        
        _archiveServiceMock.Setup(x => x.CheckAllFilesExists(_requestFiles))
            .Returns(Result.Success);
        _archiveServiceMock.Setup(x => x.LaunchArchiving(_requestFiles))
            .Returns(new ArchiveTask { ProcessId = id, WorkItem = workItem });

        await _handler.Handle(_command, CancellationToken.None);

        _processRepositoryMock.Verify(x => x.AddWithId(id, workItem), Times.Once);
    }
}