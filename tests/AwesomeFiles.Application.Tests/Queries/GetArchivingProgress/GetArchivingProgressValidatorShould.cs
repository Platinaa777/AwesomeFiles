using AwesomeFiles.Application.Queries.GetArchivingProgress;
using FluentAssertions;

namespace AwesomeFiles.Application.Tests.Queries.GetArchivingProgress;

public class GetArchivingProgressValidatorShould
{
    private readonly GetArchivingProgressQueryValidator _validator = new();
    private GetArchivingProgressQuery _query = new(1);
    
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