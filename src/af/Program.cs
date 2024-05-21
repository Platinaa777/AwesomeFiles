using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using af.Middlewares;
using af.Utils;
using Microsoft.Extensions.DependencyInjection;

namespace af;

public class Af
{
    public static async Task Main()
    {
        var rootCommand = new RootCommand("AwesomeFiles CLI - Консольная утилита для тестирования backend сервиса");

        CLICommands.RegisterHandlers(rootCommand);
        
        var builder = new CommandLineBuilder(rootCommand).UseDefaults().UseDependencyInjection(services =>
        {
            services.AddHttpClient();
        });
        
        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine();
            var inputArgs = input!.Split(' ');
            await builder.Build().InvokeAsync(inputArgs);
        }
    }
}
