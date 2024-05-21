using System.Text;
using af;

namespace AwesomeFiles.CLI.Tests;

public class UnitTest1
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