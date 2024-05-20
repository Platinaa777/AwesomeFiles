using AwesomeFiles.Application.Queries.DownloadArchive;
using AwesomeFiles.Application.Services;
using AwesomeFiles.Domain.Exceptions;
using AwesomeFiles.Domain.Models.WorkingProcessModel.Repos;
using FluentAssertions;

namespace AwesomeFiles.Application.Tests.Queries.DownloadArchive;

public class DownloadArchiveQueryShould
{
    private const long DefaultId = 1;
    private readonly DownloadArchiveQuery _query;
    private readonly Mock<IArchiveService> _archiveServiceMock = new();
    private readonly Mock<IArchiveProcessRepository> _processRepositoryMock = new();
    private readonly DownloadArchiveQueryHandler _handler;

    public DownloadArchiveQueryShould()
    {
        _handler = new(_processRepositoryMock.Object, _archiveServiceMock.Object);
        _query = new(DefaultId);
    }

    [Fact]
    public async Task ThrowArchiveNotFoundException_WhenArchiveNotFound()
    {
        _processRepositoryMock.Setup(x => x.GetById(It.IsAny<long>()))
            .Returns((Task?)null);

        await _handler.Invoking(x => x.Handle(_query, CancellationToken.None))
            .Should().ThrowAsync<ArchiveNotFoundException>();
    }

    [Fact]
    public async Task ReturnArchiveNotComplete_WhenFilesAreInArchivingProgress()
    {
        // archiving in progress
        _processRepositoryMock.Setup(x => x.GetById(It.IsAny<long>()))
            .Returns(Task.Delay(10_000));

        var result = await _handler.Handle(_query, CancellationToken.None);

        result.IsReady.Should().BeFalse();
        result.ZipBytes.Should().BeNull();
    }
    
    [Fact]
    public async Task ReturnCompletedArchive_WhenArchivingIsFinished()
    {
        var bytes = new byte[] { 1, 2, 3 };
        
        _processRepositoryMock.Setup(x => x.GetById(It.IsAny<long>()))
            .Returns(Task.CompletedTask);
        _archiveServiceMock.Setup(x => x.DownloadArchiveAsync(DefaultId))
            .ReturnsAsync(bytes);

        var result = await _handler.Handle(_query, CancellationToken.None);

        result.IsReady.Should().BeTrue();
        result.ZipBytes.Should().BeEquivalentTo(bytes);
    }
}