namespace AwesomeFiles.Domain.Models.ArchiveFileModel.Repos;

public interface IArchiveFileRepository
{
    List<ArchiveFile> GetAllFiles();
}