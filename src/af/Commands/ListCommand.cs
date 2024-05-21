using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Security.Cryptography;
using af.Utils;

namespace af.Commands;

public class ListCommand : Command
{
    private readonly IHttpClientFactory _factory;
    private const string NAME = "list";
    private const string DESC = "Показать список всех файлов";
    
    public ListCommand(IHttpClientFactory factory) : base(NAME, DESC)
    {
        _factory = factory;
        Handler = CommandHandler.Create(async () =>
        {
            using HttpClient client = _factory.CreateClient();
            var response = await client.GetAsync($"{Endpoints.BaseUrl}{Endpoints.Files}");
        
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"GET endpoint: {Endpoints.Files} response status: {response.StatusCode}");
            
            ApiResponseHandler.PrintListResult(response, content);

            return response.IsSuccessStatusCode ? 0 : -1;
        });
    }
}