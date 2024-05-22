using System.CommandLine;
using System.Net;
using System.Net.Http.Json;

namespace AwesomeFiles.CLI.Tests;

public class СreateArchiveCommandShould
{
    private readonly Mock<IHttpClientFactory> _httpFactoryMock = new();
    private readonly MockHttpMessageHandler _messageHandlerMock = new();
    private readonly CreateArchiveCommand _command;
    private const string Url = "http://localhost:5001/process/start";
    private const object NullRef = null!;

    public СreateArchiveCommandShould()
    {
        _command = new CreateArchiveCommand(_httpFactoryMock.Object);
        
        _httpFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(_messageHandlerMock));
    }
    
    [Fact]
    public async Task ReturnNegativeStatusCode_WhenHttpResponseFailure()
    {
        _messageHandlerMock.When(Url).Respond(HttpStatusCode.BadRequest, JsonContent.Create(new {
            success = false,
            body = NullRef,
            errors = new[] {
                "File 'file3' does not exists in the system"
            }
        }));

        var exitCode = await _command.InvokeAsync(new[] { "file1" });

        exitCode.Should().Be(-1);
    }
    
    [Fact]
    public async Task ReturnProcessId_WhenHttpResponseIsSuccessfullyCreateArchiveTask()
    {
        _messageHandlerMock.When(Url).Respond(HttpStatusCode.OK, JsonContent.Create(new {
            success = true,
            body =  new {
                id = 1
            },
            errors = NullRef
        }));

        var exitCode = await _command.InvokeAsync(new[] { "file1" });

        exitCode.Should().Be(1);
    }
}