using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using af.Utils;
using AwesomeFiles.HttpModels.Responses;
using Newtonsoft.Json;

namespace af.Commands;

public class StatusCommand : Command
{
    private readonly IHttpClientFactory _factory;
    private const string NAME = "status";
    private const string DESC = "Проверить статус процесса";
    
    public StatusCommand(IHttpClientFactory factory) : base(NAME, DESC)
    {
        _factory = factory;
        AddArgument(new Argument<int>("taskId", "Id процесса который надо проверить на готовность"));
        
        Handler = CommandHandler.Create<int>(async taskId =>
        {
            using HttpClient client = _factory.CreateClient();
            var response = await client.GetAsync($"{Endpoints.BaseUrl}/process/{taskId}");
            
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
    }
}