namespace AwesomeFiles.Domain.Models.ArchiveFileModel.Repos;

public interface IArchiveFileRepository
{
    Task<ArchiveFile> GetAllFiles();
    Task<ArchiveFile?> GetFileByPath();
    Task<bool> Add();
}