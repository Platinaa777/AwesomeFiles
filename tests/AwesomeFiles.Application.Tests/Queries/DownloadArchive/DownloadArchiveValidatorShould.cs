using AwesomeFiles.Application.Queries.DownloadArchive;
using FluentAssertions;

namespace AwesomeFiles.Application.Tests.Queries.DownloadArchive;

public class DownloadArchiveValidatorShould
{
    private readonly DownloadArhiveQueryValidator _validator = new();
    private DownloadArchiveQuery _query = new(1);
    
    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(1_000_000)]
    public void ReturnSuccess_WhenQueryValid(long id)
    {
        _query = new(id);
        _validator.Validate(_query).IsValid.Should().BeTrue();
    }
    
    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(-100)]
    public void ReturnFailure_WhenQueryInvalid(long id)
    {
        _query = new(id);
        _validator.Validate(_query).IsValid.Should().BeFalse();
    }
}