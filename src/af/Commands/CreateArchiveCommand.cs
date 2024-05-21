using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Text;
using af.Utils;
using AwesomeFiles.HttpModels.Responses;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace af.Commands;

public class CreateArchiveCommand : Command
{
    private readonly IHttpClientFactory _factory;
    private const string NAME = "create-archive";
    private const string DESC = "Создать архив с введеными файлами";
    
    public CreateArchiveCommand(IHttpClientFactory factory) : base(NAME, DESC)
    {
        _factory = factory;
        AddArgument(new Argument<string[]>("files", "Файлы которые должны быть архивированы"));
        
        Handler = CommandHandler.Create<string[]>(async files =>
        {
            using HttpClient client = _factory.CreateClient();
            
            string jsonData = JsonSerializer.Serialize(new { FileNames = files });
            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{Endpoints.BaseUrl}{Endpoints.ArchiveStart}", content);
            
            string responseContent = await response.Content.ReadAsStringAsync();
            
            Console.WriteLine($"POST endpoint: {Endpoints.ArchiveStart} request status: {response.StatusCode}");
            ApiResponseHandler.PrintCreateArchiveResult(response, responseContent);

            int exitCode = -1;
            if (response.IsSuccessStatusCode)
                exitCode = (int)JsonConvert.DeserializeObject<ApiResponse<ProcessIdResponse>>(responseContent)!.Body!.Id;

            return exitCode;
        });
    }
}