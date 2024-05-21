using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Text;
using AwesomeFiles.HttpModels.Responses;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace af.Utils;

public static class CLICommands
{
    public static readonly Command ListCommand = new("list", "Показать список всех файлов");

    private static readonly Command DownloadCommand = new("download", "Скачать архив c id процесса")
    {
        new Argument<int>("taskId", "Id процесса"),
        new Argument<string>("path", "Путь куда будет скачан архив")
    };

    private static readonly Command StatusCommand = new("status", "Проверить статус процесса")
    {
        new Argument<int>("taskId", "Id процесса который надо проверить на готовность")
    };

    private static readonly Command ExitCommand = new("exit", "Выйти с приложения");

    private static readonly Command CreateArchiveCommand = new("create-archive", "Создать архив с введеными файлами")
    {
        new Argument<string[]>("files", "Файлы которые должны быть архивированы")
    };
    
    private static readonly Command AutoCheckingCommand = new("auto-create-archive", "Режим который самостоятельно запрашивает у backend создание архива, опрашивание о его готовности и скачивание (опрос происходит каждые 200ms)")
    {
        new Argument<string>("path", "Путь куда будет скачан архив"),
        new Argument<string[]>("files", "Файлы которые должны быть архивированы")
    };

    private const string BaseUrl = "http://localhost:5001";

    public static void RegisterHandlers(Command rootCommand)
    {
        ListCommand.Handler = CommandHandler.Create(async () =>
        {
            using HttpClient client = new HttpClient();
            var response = await client.GetAsync($"{BaseUrl}/files");
        
            string content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"GET response status: {response.StatusCode}");
            Console.WriteLine(content);

            return response.IsSuccessStatusCode ? 0 : -1;
        });
        
        CreateArchiveCommand.Handler = CommandHandler.Create<string[]>(async files =>
        {
            using HttpClient client = new HttpClient();
            string jsonData = JsonSerializer.Serialize(new { FileNames = files });
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{BaseUrl}/process/start", content);
            
            string responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"POST request status: {response.StatusCode}");
            Console.WriteLine(responseContent);

            int exitCode = -1;
            if (response.IsSuccessStatusCode)
                exitCode = (int)JsonConvert.DeserializeObject<ProcessIdResponse>(responseContent)!.Id;

            return exitCode;
        });

        StatusCommand.Handler = CommandHandler.Create<int>(async taskId =>
        {
            using HttpClient client = new HttpClient();
            var response = await client.GetAsync($"{BaseUrl}/process/{taskId}");
            
            string content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"GET response status: {response.StatusCode}");
            Console.WriteLine(content);
            
            if (!response.IsSuccessStatusCode)
                return -1;
            
            string status = "Pending";
            if (response.IsSuccessStatusCode)
                status = JsonConvert.DeserializeObject<ArchivingStatus>(content)!.Status;

            return status != "Pending" ? taskId : 0;
        });

        ExitCommand.Handler = CommandHandler.Create(() =>
        {
            Environment.Exit(0);
        });

        DownloadCommand.Handler = CommandHandler.Create<int, string>(async (taskId, path) =>
        {
            using HttpClient client = new HttpClient();
            var response = await client.GetAsync($"{BaseUrl}/process/download/{taskId}");
            
            Console.WriteLine($"GET response status: {response.StatusCode}");
            if (response.IsSuccessStatusCode)
            {
                byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
                string filePath = Path.Combine(path, $"archive-{taskId}.zip");
                await File.WriteAllBytesAsync(filePath, fileBytes);
                Console.WriteLine($"File downloaded successfully to {filePath}");
            }
            
            return response.IsSuccessStatusCode ? taskId : -1;
        });
        
        AutoCheckingCommand.Handler = CommandHandler.Create<string, string[]>(async (path,files ) =>
        {
            using HttpClient client = new HttpClient();

            var processId = await CreateArchiveCommand.InvokeAsync(files);

            // Не смогли создать процесс на архивацию
            if (processId == -1)
                return;

            var isReady = false;
            do
            {
                var statusCommandExitCode = await StatusCommand.InvokeAsync(new []{ processId.ToString() });
                if (statusCommandExitCode == processId)
                    isReady = true;
                await Task.Delay(200);
            } while (!isReady);
        
            await DownloadCommand.InvokeAsync(new []{ processId.ToString(), path});
        });
        
        rootCommand.AddCommand(ListCommand);        
        rootCommand.AddCommand(DownloadCommand);
        rootCommand.AddCommand(StatusCommand);
        rootCommand.AddCommand(ExitCommand);
        rootCommand.AddCommand(CreateArchiveCommand);
        rootCommand.AddCommand(AutoCheckingCommand);
    }
}