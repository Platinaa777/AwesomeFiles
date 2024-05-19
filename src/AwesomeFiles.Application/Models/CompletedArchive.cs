namespace AwesomeFiles.Application.Models;

public class CompletedArchive
{
    public CompletedArchive(bool isReady)
    {
        IsReady = isReady;
    }

    public CompletedArchive(MemoryStream stream, bool isReady) : this(isReady)
    {
        Stream = stream;
        IsReady = isReady;
    }
    public bool IsReady { get; set; }
    public MemoryStream Stream { get; set; } = null!;
}