namespace AwesomeFiles.Domain.Models.ArchiveFileModel;

public class AwesomeFile
{
    public AwesomeFile(string name)
    {
        Name = name;
    }
    
    public string Name { get; init; }
}