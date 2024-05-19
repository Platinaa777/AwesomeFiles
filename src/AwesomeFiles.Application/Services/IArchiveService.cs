using AwesomeFiles.Application.Services.Models;
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
    Result CheckAllFilesExists(List<string> files);
    /// <summary>
    /// Запус архивации
    /// </summary>
    /// <param name="existingFiles">Файлы находящиеся на устройстве</param>
    /// <returns></returns>
    ArchiveTask LaunchArchiving(List<string> existingFiles);

    /// <summary>
    /// Скачивание из архива и возвращение потока
    /// </summary>
    /// <param name="archiveName">Имя архива</param>
    /// <returns></returns>
    MemoryStream DownloadArchive(long processId);
}