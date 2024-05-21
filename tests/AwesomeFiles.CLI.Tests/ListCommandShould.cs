using System.CommandLine;
using System.Net;
using System.Net.Http.Json;

namespace AwesomeFiles.CLI.Tests;

public class ListCommandShould
{
    private readonly Mock<IHttpClientFactory> _httpFactoryMock = new();
    private readonly MockHttpMessageHandler _messageHandlerMock = new();
    private readonly ListCommand _command;
    private const string Url = "http://localhost:5001/files";
    private const object NullRef = null!;
    
    public ListCommandShould()
    {
        _command = new ListCommand(_httpFactoryMock.Object);
        _httpFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(_messageHandlerMock));
    }

    [Fact]
    public async Task ReturnNegativeExitCode_WhenHttpResponseIsFailed()
    {
        _messageHandlerMock.When(Url).Respond(HttpStatusCode.BadRequest, JsonContent.Create(new { smth = "empty" }));

        var exitCode = await _command.InvokeAsync(Enumerable.Empty<string>().ToArray());

        exitCode.Should().Be(-1);
    }

    [Fact]
    public async Task ReturnZero_WhenSuccessfullyFetchedFilesFromBackend()
    {
        _messageHandlerMock.When(Url).Respond(HttpStatusCode.OK, JsonContent.Create(new
        {
            success = true,
            body = new object[] {
                new { name = "file2" },
                new { name = "file1" }
            },
            errors = NullRef
        }));

        var exitCode = await _command.InvokeAsync(Enumerable.Empty<string>().ToArray());

        exitCode.Should().Be(0);
    }
}