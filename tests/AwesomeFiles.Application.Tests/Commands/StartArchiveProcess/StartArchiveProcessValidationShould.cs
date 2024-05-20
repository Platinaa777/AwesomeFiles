using AwesomeFiles.Application.Commands.StartArchiveProcess;
using FluentAssertions;

namespace AwesomeFiles.Application.Tests.Commands.StartArchiveProcess;

public class StartArchiveProcessValidationShould
{
    private readonly StartArchiveProcessCommandValidator _validator = new();

    public static IEnumerable<object[]> GetValidCommands()
    {
        var validCommand = new StartArchiveProcessCommand(new List<string> { "file1", "file2" });
        yield return new object[] { validCommand } ;
        yield return new object[] { validCommand with { Files = new() { "file1" } } };
        yield return new object[] { validCommand with { Files = Enumerable.Range(0, 100).Select(x => x.ToString()).ToList() } };
    }
    
    [Theory]
    [MemberData(nameof(GetValidCommands))]
    public void ReturnSuccess_WhenCommandValid(StartArchiveProcessCommand command)
    {
        _validator.Validate(command).IsValid.Should().BeTrue();
    }

    [Fact]
    public void ReturnFailure_WhenCommandInvalid()
    {
        var command = new StartArchiveProcessCommand(Enumerable.Empty<string>().ToList());
        _validator.Validate(command).IsValid.Should().BeFalse();
    }
}