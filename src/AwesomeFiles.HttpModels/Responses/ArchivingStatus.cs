namespace AwesomeFiles.HttpModels.Responses;

public class ArchivingStatus
{
    public ArchivingStatus(string status)
    {
        Status = status;
    }

    public string Status { get; set; }
}