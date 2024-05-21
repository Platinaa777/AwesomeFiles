using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Text;
using AwesomeFiles.HttpModels.Responses;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace af.Utils;

public static class CLICommands
{
    private static readonly Command ListCommand = new("list", "Показать список всех файлов");

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
        ListCommand.Handler = CommandHandler.Create(async (IHttpClientFactory factory) =>
        {
            using HttpClient client = factory.CreateClient();
            Console.WriteLine(factory.ToString());
            var response = await client.GetAsync($"{BaseUrl}{Endpoints.Files}");
        
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"GET endpoint: {Endpoints.Files} response status: {response.StatusCode}");
            
            ApiResponseHandler.PrintListResult(response, content);

            return response.IsSuccessStatusCode ? 0 : -1;
        });
        
        CreateArchiveCommand.Handler = CommandHandler.Create<string[], IHttpClientFactory>(async (files, factory) =>
        {
            using HttpClient client = factory.CreateClient();
            
            string jsonData = JsonSerializer.Serialize(new { FileNames = files });
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{BaseUrl}{Endpoints.ArchiveStart}", content);
            
            string responseContent = await response.Content.ReadAsStringAsync();
            
            Console.WriteLine($"POST endpoint: {Endpoints.ArchiveStart} request status: {response.StatusCode}");
            ApiResponseHandler.PrintCreateArchiveResult(response, responseContent);

            int exitCode = -1;
            if (response.IsSuccessStatusCode)
                exitCode = (int)JsonConvert.DeserializeObject<ApiResponse<ProcessIdResponse>>(responseContent)!.Body!.Id;

            return exitCode;
        });

        StatusCommand.Handler = CommandHandler.Create<int, IHttpClientFactory>(async (taskId, factory) =>
        {
            using HttpClient client = factory.CreateClient();
            var response = await client.GetAsync($"{BaseUrl}/process/{taskId}");
            
            string content = await response.Content.ReadAsStringAsync();
            
            Console.WriteLine($"GET endpoint: /process/{taskId} response status: {response.StatusCode}");
            ApiResponseHandler.PrintStatusArchivingResult(response, content);
            
            if (!response.IsSuccessStatusCode)
                return -1;
            
            string status = "Pending";
            if (response.IsSuccessStatusCode)
                status = JsonConvert.DeserializeObject<ApiResponse<ArchivingStatus>>(content)!.Body!.Status;

            return status != "Pending" ? taskId : 0;
        });

        ExitCommand.Handler = CommandHandler.Create(() =>
        {
            Environment.Exit(0);
        });

        DownloadCommand.Handler = CommandHandler.Create<int, string, IHttpClientFactory>(async (taskId, path, factory) =>
        {
            using HttpClient client = factory.CreateClient();
            var response = await client.GetAsync($"{BaseUrl}/process/download/{taskId}");
            
            Console.WriteLine($"GET endpoint: /process/download/{taskId} response status: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
                string filePath = Path.Combine(path, $"archive-{taskId}.zip");
                await File.WriteAllBytesAsync(filePath, fileBytes);
                Console.WriteLine($"File downloaded successfully to {filePath}");
            }
            else
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            
            return response.IsSuccessStatusCode ? taskId : -1;
        });
        
        AutoCheckingCommand.Handler = CommandHandler.Create<string, string[]>(async (path,files ) =>
        {
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