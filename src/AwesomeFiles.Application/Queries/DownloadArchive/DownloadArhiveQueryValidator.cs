using FluentValidation;

namespace AwesomeFiles.Application.Queries.DownloadArchive;

public class DownloadArhiveQueryValidator
    : AbstractValidator<DownloadArchiveQuery>
{
    public DownloadArhiveQueryValidator()
    {
        RuleFor(x => x.ProcessId).GreaterThan(0)
            .WithMessage("Id процесса не может быть меньше или равно 0");
    }
}