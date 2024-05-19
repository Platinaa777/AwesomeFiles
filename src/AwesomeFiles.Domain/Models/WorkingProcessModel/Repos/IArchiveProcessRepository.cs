namespace AwesomeFiles.Domain.Models.WorkingProcessModel.Repos;

public interface IArchiveProcessRepository
{
    void AddWithId(long id, Task archivingProcess);
    Task? GetById(long id);
}