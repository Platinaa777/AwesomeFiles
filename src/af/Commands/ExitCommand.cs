using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

namespace af.Commands;

public class ExitCommand : Command
{
    private const string NAME = "exit";
    private const string DESC = "Выйти с приложения";
    
    public ExitCommand(string name = NAME, string? description = DESC) : base(name, description)
    {
        Handler = CommandHandler.Create(() =>
        {
            Environment.Exit(0);
        });
    }
}