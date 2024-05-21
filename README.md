# Мирошниченко Денис

telegram: _@platina_777_

Github: _https://github.com/Platinaa777_

## Утилита af

### af (AwesomeFiles)

## Запуск:

```cs
dotnet af.dll
```

## Описание:

```csharp
Description:
  AwesomeFiles CLI - Консольная утилита для тестирования backend сервиса

Usage:
  af [command] [options]

Options:
  --version       Show version information
  -?, -h, --help  Show help and usage information

Commands:
  auto-create-archive <path> <files>  Режим который самостоятельно запрашивает у backend создание архива, опрашивание о его готовности и скачивание (опрос происходит каждые 200ms)
  create-archive <files>              Создать архив с введеными файлами
  download <taskId> <path>            Скачать архив c id процесса
  exit                                Выйти с приложения
  list                                Показать список всех файлов
  status <taskId>                     Проверить статус процесса

```

## Команды:

```csharp
> auto-create-archive -h
Description:
  Режим который самостоятельно запрашивает у backend создание архива, опрашивание о его готовности и скачивание (опрос происходит каждые 200ms)

Usage:
  af auto-create-archive <path> [<files>...] [options]

Arguments:
  <path>   Путь куда будет скачан архив
  <files>  Файлы которые должны быть архивированы
----------------------------------------------------------------------------------------------
> create-archive -h
Description:
  Создать архив с введеными файлами

Usage:
  af create-archive [<files>...] [options]

Arguments:
  <files>  Файлы которые должны быть архивированы
-----------------------------------------------------------------------------------------------
> download -h
Description:
  Скачать архив c id процесса

Usage:
  af download <taskId> <path> [options]

Arguments:
  <taskId>  Id процесса
  <path>    Путь куда будет скачан архив
------------------------------------------------------------------------------------------------
> exit -h
Description:
  Выйти с приложения

Usage:
  af exit [options]
-------------------------------------------------------------------------------------------------
> list -h
Description:
  Показать список всех файлов

Usage:
  af list [options]
--------------------------------------------------------------------------------------------------
> status -h
Description:
  Проверить статус процесса

Usage:
  af status <taskId> [options]

Arguments:
  <taskId>  Id процесса который надо проверить на готовность

```

## Использование:

```csharp
Запускаем наш backend сервис (после этого запуск утилиты)

❯ dotnet run
> list
GET endpoint: /files response status: OK
file2 file1 
> create-archive file1 file2
POST endpoint: /process/start request status: OK
Создана задача на архивацию, id: 1
> status 1
GET endpoint: /process/1 response status: OK
Задача на архивацию находится в статусе: Pending
> download 1 ./temp
GET endpoint: /process/download/1 response status: OK
File downloaded successfully to ./temp/archive-1.zip
```
