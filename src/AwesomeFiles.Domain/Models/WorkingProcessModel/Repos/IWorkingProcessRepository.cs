namespace AwesomeFiles.Domain.Models.WorkingProcessModel.Repos;

public interface IWorkingProcessRepository
{
    Task Add();
    Task<WorkingProcess?> GetById();
}