namespace AwesomeFiles.HttpModels.Responses;

public class StartArchivingResponse
{
    public StartArchivingResponse(List<string> errors)
    {
        Errors = errors;
    }

    public List<string> Errors { get; set; }
}