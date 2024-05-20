using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Net;
using System.Text;
using System.Text.Json;

namespace af.Utils;

public static class CLICommands
{
    public static readonly Command ListCommand = new("list", "List files");

    private static readonly Command DownloadCommand = new("download", "Download the result of a task to a specified folder")
    {
        new Argument<int>("taskId", "The ID of the task to download"),
        new Argument<string>("path", "The path to the folder where the file should be downloaded")
    };

    private static readonly Command StatusCommand = new("status", "Check the status of a task")
    {
        new Argument<int>("taskId", "The ID of the task to check")
    };

    private static readonly Command ExitCommand = new("exit", "Exit the application");

    private static readonly Command CreateArchiveCommand = new("create-archive", "Create an archive from specified files")
    {
        new Argument<string[]>("files", "Files to include in the archive"),
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
        });

        StatusCommand.Handler = CommandHandler.Create<int>(async taskId =>
        {
            using HttpClient client = new HttpClient();
            var response = await client.GetAsync($"{BaseUrl}/process/{taskId}");
            
            string content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"GET response status: {response.StatusCode}");
            Console.WriteLine(content);
        });

        ExitCommand.Handler = CommandHandler.Create(() =>
        {
            Environment.Exit(0);
        });

        DownloadCommand.Handler = CommandHandler.Create<int, string>(async (taskId, path) =>
        {
            using HttpClient client = new HttpClient();
            var response = await client.GetAsync($"{BaseUrl}/process/download/{taskId}");
            
            string content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"GET response status: {response.StatusCode}");
            if (response.IsSuccessStatusCode)
            {
                byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
                string filePath = Path.Combine(path, $"archive-{taskId}.zip");
                await File.WriteAllBytesAsync(filePath, fileBytes);
                Console.WriteLine($"File downloaded successfully to {filePath}");
            }
        });
        
        rootCommand.AddCommand(ListCommand);        
        rootCommand.AddCommand(DownloadCommand);
        rootCommand.AddCommand(StatusCommand);
        rootCommand.AddCommand(ExitCommand);
        rootCommand.AddCommand(CreateArchiveCommand);
    }
}