using AwesomeFiles.Domain.Models.ArchiveFileModel.Repos;
using MediatR;

namespace AwesomeFiles.Application.Query.GetStorageFiles;

public class GetStorageFilesQueryHandler
    : IRequestHandler<GetStorageFilesQuery, List<GetStorageFileModel>>
{
    private readonly IArchiveFileRepository _archiveFileRepository;

    public GetStorageFilesQueryHandler(
        IArchiveFileRepository archiveFileRepository)
    {
        _archiveFileRepository = archiveFileRepository;
    }

    public Task<List<GetStorageFileModel>> Handle(GetStorageFilesQuery request, CancellationToken cancellationToken)
        => Task.FromResult(_archiveFileRepository.GetAllFiles()
            .Select(x => new GetStorageFileModel(x.Name)).ToList());
}