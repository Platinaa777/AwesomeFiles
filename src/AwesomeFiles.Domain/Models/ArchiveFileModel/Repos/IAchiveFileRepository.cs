namespace AwesomeFiles.Domain.Models.ArchiveFileModel.Repos;

public interface IFileRepository
{
    List<AwesomeFile> GetAllFiles();
}