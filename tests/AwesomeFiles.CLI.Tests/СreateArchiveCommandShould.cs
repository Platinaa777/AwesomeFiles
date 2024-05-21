using System.Text;
using af;

namespace AwesomeFiles.CLI.Tests;

public class СreateArchiveCommandShould
{
    [Fact]
    public async Task Test1()
    {
        Console.SetIn(new StringReader("h"));
        var st = new StringBuilder();
        Console.SetOut(new StringWriter(st));
        await Af.Main();

        int x = 5;
    }
}