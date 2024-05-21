namespace AwesomeFiles.HttpModels.Responses;

public class FileInfoResponse
{
    public FileInfoResponse(string name)
    {
        Name = name;
    }

    public string Name { get; set; }
}