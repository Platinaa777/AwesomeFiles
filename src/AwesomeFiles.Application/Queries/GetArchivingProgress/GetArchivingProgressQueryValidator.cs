using FluentValidation;

namespace AwesomeFiles.Application.Queries.GetArchivingProgress;

public class GetArchivingProgressQueryValidator
    : AbstractValidator<GetArchivingProgressQuery>
{
    public GetArchivingProgressQueryValidator()
    {
        RuleFor(x => x.ProcessId).GreaterThan(0)
            .WithMessage("Id процесса не может быть меньше или равно 0");
    }
}