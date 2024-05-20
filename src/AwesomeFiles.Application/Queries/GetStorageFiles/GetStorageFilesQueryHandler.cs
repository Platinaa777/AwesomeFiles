using AwesomeFiles.Domain.Models.ArchiveFileModel.Repos;
using MediatR;

namespace AwesomeFiles.Application.Queries.GetStorageFiles;

public class GetStorageFilesQueryHandler
    : IRequestHandler<GetStorageFilesQuery, List<GetStorageFileModel>>
{
    private readonly IFileRepository _fileRepository;

    public GetStorageFilesQueryHandler(
        IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }

    public Task<List<GetStorageFileModel>> Handle(GetStorageFilesQuery request, CancellationToken cancellationToken)
        => Task.FromResult(_fileRepository.GetAllFiles()
            .Select(x => new GetStorageFileModel(x.Name)).ToList());
}