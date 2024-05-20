using System.CommandLine;
using af.Utils;

namespace af;

static class Program
{
    static async Task Main()
    {
        var rootCommand = new RootCommand("AwesomeFiles CLI - Консольная утилита для тестирования backend сервиса");

        CLICommands.RegisterHandlers(rootCommand);

        while (true)
        {
            // Перехват исключения, например backend не запущен => не сможем отправить HTTP запрос
            Console.Write("> ");
            var input = Console.ReadLine();
            var inputArgs = input!.Split(' ');
            await rootCommand.InvokeAsync(inputArgs);
        }
    }
}
