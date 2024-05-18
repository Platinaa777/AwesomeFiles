using FluentValidation;

namespace AwesomeFiles.Application.Commands.StartArchiveProcess;

public class StartArchiveProcessCommandValidator
    : AbstractValidator<StartArchiveProcessCommand>
{
    public StartArchiveProcessCommandValidator()
    {
        RuleFor(x => x.Files).NotEmpty();
    }
}