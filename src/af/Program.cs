using System.CommandLine;
using af.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace af;

public class Af
{
    public static async Task Main()
    {
        var rootCommand = new RootCommand("AwesomeFiles CLI - Консольная утилита для тестирования backend сервиса");

        var services = new ServiceCollection();
        DependencyInjection.ConfigureServices(services);

        await using var serviceProvider = services.BuildServiceProvider();

        var commands = serviceProvider.GetServices<Command>();
        commands.ToList().ForEach(rootCommand.AddCommand);

        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine();
            var inputArgs = input!.Split(' ');
            await rootCommand.InvokeAsync(inputArgs);
        }
    }
}
