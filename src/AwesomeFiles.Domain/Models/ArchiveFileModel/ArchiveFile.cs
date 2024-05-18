namespace AwesomeFiles.Domain.Models.ArchiveFileModel;

public class ArchiveFile
{
    public ArchiveFile(string id, string path)
    {
        Id = id;
        Path = path;
    }
    
    public string Id { get; init; }
    public string Path { get; init; }
}