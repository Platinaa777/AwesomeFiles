namespace AwesomeFiles.Domain.Exceptions;

public class ArchiveNotFoundException : Exception
{
    public ArchiveNotFoundException()
    {
    }

    public ArchiveNotFoundException(string? message) : base(message)
    {
    }

    public ArchiveNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}