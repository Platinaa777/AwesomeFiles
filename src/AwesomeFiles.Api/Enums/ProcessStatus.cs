using AwesomeFiles.HttpModels.Responses;

namespace AwesomeFiles.Api.Enums;

public enum ProcessStatus
{
    Pending = 1,
    Completed = 2
}

public static class ProcessUtils
{
    public static ArchivingStatus ConvertToApiResponse(bool commandResult) => commandResult switch
    {
        true => new ArchivingStatus(ProcessStatus.Completed.ToString()),
        false => new ArchivingStatus(ProcessStatus.Pending.ToString())
    };
}
