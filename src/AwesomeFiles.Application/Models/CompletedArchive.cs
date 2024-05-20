namespace AwesomeFiles.Application.Models;

public class CompletedArchive
{
    public CompletedArchive(bool isReady)
    {
        IsReady = isReady;
    }

    public CompletedArchive(byte[] zipBytes, bool isReady) : this(isReady)
    {
        ZipBytes = zipBytes;
        IsReady = isReady;
    }
    public bool IsReady { get; set; }
    public byte[] ZipBytes { get; set; } = null!;
}