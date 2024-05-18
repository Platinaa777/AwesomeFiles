using AwesomeFiles.Domain.Models.WorkingProcessModel;
using AwesomeFiles.Domain.ResultAbstractions;

namespace AwesomeFiles.Application.Services;

/// <summary>
/// Абстракция над архиватором для быстрой замены
/// (вдруг все-таки решим хранить где-то не только на локали :) )
/// </summary>
public interface IArchiveService
{
    /// <summary>
    /// Проверка на то что все файлы существуют на устройстве
    /// </summary>
    /// <returns></returns>
    Result CheckFiles(List<string> files);
    /// <summary>
    /// Запус архивации
    /// </summary>
    /// <param name="files"></param>
    /// <returns></returns>
    WorkingProcess LaunchArchiving(List<string> files);
    /// <summary>
    /// Скачивание из архива и возвращение потока
    /// </summary>
    /// <param name="process"></param>
    /// <returns></returns>
    dynamic DownloadArchive(WorkingProcess process);
}