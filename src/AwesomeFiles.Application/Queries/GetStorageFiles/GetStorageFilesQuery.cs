using MediatR;

namespace AwesomeFiles.Application.Queries.GetStorageFiles;

/// <summary>
/// Решил что лучше все-таки вынести это как отдельный запрос (query), а не inject'ить все в контроллер,
/// с расчетом на то что в приложении может быть пагинация
/// => будет легко добавить просто параметры skip take (мб какая-то еще логика)
/// </summary>
public record GetStorageFilesQuery : IRequest<List<GetStorageFileModel>>;
