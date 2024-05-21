using System.CommandLine;

namespace af;

public class ACommand : Command
{
    public ACommand(string name, string? description = null) : base(name, description)
    {
    }
}