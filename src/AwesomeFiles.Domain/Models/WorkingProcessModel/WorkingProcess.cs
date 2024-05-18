using System.Diagnostics;

namespace AwesomeFiles.Domain.Models.WorkingProcessModel;

public class WorkingProcess
{
    public WorkingProcess(int id, Guid operationId, Process currentProcess)
    {
        Id = id;
        CurrentProcess = currentProcess;
        OperationId = operationId;
    }

    public int Id { get; init; }
    public Guid OperationId { get; set; }
    public Process CurrentProcess { get; private set; }
}