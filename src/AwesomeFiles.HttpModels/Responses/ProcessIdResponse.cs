namespace AwesomeFiles.HttpModels.Responses;

public class ProcessIdResponse
{
    public ProcessIdResponse(long id)
    {
        Id = id;
    }

    public long Id { get; set; }
}