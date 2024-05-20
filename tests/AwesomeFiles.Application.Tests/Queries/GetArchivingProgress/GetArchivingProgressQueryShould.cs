using AwesomeFiles.Application.Queries.GetArchivingProgress;
using AwesomeFiles.Domain.Exceptions;
using AwesomeFiles.Domain.Models.WorkingProcessModel.Repos;
using FluentAssertions;


namespace AwesomeFiles.Application.Tests.Queries.GetArchivingProgress;

public class GetArchivingProgressQueryShould
{
    private const long DefaultId = 1;
    private readonly GetArchivingProgressQuery _query;
    private readonly Mock<IArchiveProcessRepository> _archiveProcessRepositoryMock = new();
    private readonly GetArchivingProgressQueryHandler _handler;

    public GetArchivingProgressQueryShould()
    {
        _handler = new(_archiveProcessRepositoryMock.Object);
        _query = new(DefaultId);
    }

    [Fact]
    public async Task ThrowArchiveNotFoundException_WhenArchiveNotFound()
    {
        _archiveProcessRepositoryMock.Setup(x => x.GetById(It.IsAny<long>()))
            .Returns((Task?)null);

        await _handler.Invoking(x => x.Handle(_query, CancellationToken.None))
            .Should().ThrowAsync<ArchiveNotFoundException>();
    }
    
    [Fact]
    public async Task ReturnFalse_WhenTaskArchivingNotCompleted()
    {
        _archiveProcessRepositoryMock.Setup(x => x.GetById(It.IsAny<long>()))
            .Returns(Task.Delay(10_000));

        var result = await _handler.Handle(_query, CancellationToken.None);

        result.Should().BeFalse();
    }
    
    [Fact]
    public async Task ReturnTrue_WhenTaskArchivingIsFinished()
    {
        _archiveProcessRepositoryMock.Setup(x => x.GetById(It.IsAny<long>()))
            .Returns(Task.CompletedTask);

        var result = await _handler.Handle(_query, CancellationToken.None);

        result.Should().BeTrue();
    }
    
    [Fact]
    public async Task ThrowExceptionFromArchivingTask_WhenWhileArchivingOccuredException()
    {
        _archiveProcessRepositoryMock.Setup(x => x.GetById(It.IsAny<long>()))
            .Returns(Task.FromException(new Exception()));

        await _handler.Invoking(x => x.Handle(_query, CancellationToken.None))
            .Should().ThrowAsync<Exception>();
    }
}