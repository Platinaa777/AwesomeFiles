namespace AwesomeFiles.Domain.Models.ArchiveFileModel;

public class ArchiveFile
{
    public ArchiveFile(string name)
    {
        Name = name;
    }
    
    public string Name { get; init; }
}