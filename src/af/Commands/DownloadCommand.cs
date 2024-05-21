using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Security.Cryptography;
using af.Services;
using af.Utils;

namespace af.Commands;

public class DownloadCommand : Command
{
    private readonly IHttpClientFactory _factory;
    private readonly IFileService _fileService;
    private const string NAME = "download";
    private const string DESC = "Скачать архив c id процесса";
    
    public DownloadCommand(IHttpClientFactory factory, IFileService fileService) : base(NAME, DESC)
    {
        _factory = factory;
        _fileService = fileService;
        AddArgument(new Argument<int>("taskId", "Id процесса"));
        AddArgument(new Argument<string>("path", "Путь куда будет скачан архив"));
        
        Handler = CommandHandler.Create<int, string>(async (taskId, path) =>
        {
            using HttpClient client = _factory.CreateClient();
            var response = await client.GetAsync($"{Endpoints.BaseUrl}/process/download/{taskId}");
            
            Console.WriteLine($"GET endpoint: /process/download/{taskId} response status: {response.StatusCode}");
            
            if (response.IsSuccessStatusCode)
            {
                byte[] fileBytes = await response.Content.ReadAsByteArrayAsync();
                string filePath = Path.Combine(path, $"archive-{taskId}.zip");
                await _fileService.WriteAllBytesAsync(filePath, fileBytes);
                Console.WriteLine($"File downloaded successfully to {filePath}");
            }
            else
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            
            return response.IsSuccessStatusCode ? taskId : -1;
        });
    }
}