using AwesomeFiles.Application.Queries.GetStorageFiles;
using AwesomeFiles.Domain.Models.ArchiveFileModel;
using AwesomeFiles.Domain.Models.ArchiveFileModel.Repos;
using FluentAssertions;

namespace AwesomeFiles.Application.Tests.Queries.GetStorageFiles;

public class GetStorageFilesQueryShould
{
    private readonly GetStorageFilesQuery _query;
    private readonly Mock<IFileRepository> _fileRepositoryMock = new();
    private readonly GetStorageFilesQueryHandler _handler;

    public GetStorageFilesQueryShould()
    {
        _handler = new(_fileRepositoryMock.Object);
        _query = new();
    }

    [Fact]
    public async Task ReturnEmptyResult_WhenNoFilesInStorage()
    {
        _fileRepositoryMock.Setup(x => x.GetAllFiles())
            .Returns(new List<AwesomeFile>());
        
        var result = await _handler.Handle(_query, CancellationToken.None);

        result.Should().BeEmpty();
    }
    
    [Fact]
    public async Task ReturnFiles_WhenFilesExistsInStorage()
    {
        _fileRepositoryMock.Setup(x => x.GetAllFiles())
            .Returns(new List<AwesomeFile> { new("file1"), new("file2")});
        List<GetStorageFileModel> expectedResponse = new List<GetStorageFileModel>
        {
            new("file1"), new("file2")
        };
        
        var result = await _handler.Handle(_query, CancellationToken.None);

        result.Should().NotBeNullOrEmpty();
        result.Should().BeEquivalentTo(expectedResponse);
    }
}