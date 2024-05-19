namespace AwesomeFiles.Domain.Exceptions;

public class ArchiveFileException : Exception
{
    public ArchiveFileException()
    {
    }

    public ArchiveFileException(string? message) : base(message)
    {
    }

    public ArchiveFileException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}