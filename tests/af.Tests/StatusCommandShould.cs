using System.CommandLine;
using System.Net;
using System.Net.Http.Json;

namespace AwesomeFiles.CLI.Tests;

public class StatusCommandShould
{
    private readonly Mock<IHttpClientFactory> _httpFactoryMock = new();
    private readonly MockHttpMessageHandler _messageHandlerMock = new();
    private readonly StatusCommand _command;
    private const string Id = "1";
    private const string Url = $"http://localhost:5001/process/{Id}";
    private const object NullRef = null!;
    
    public StatusCommandShould()
    {
        _command = new StatusCommand(_httpFactoryMock.Object);
        _httpFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(_messageHandlerMock));
    }

    [Fact]
    public async Task ReturnNegativeExitCode_WhenHttpResponseIsFailed()
    {
        _messageHandlerMock.When(Url).Respond(HttpStatusCode.BadRequest, JsonContent.Create(
            new { smth = "empty" }
        ));

        var exitCode = await _command.InvokeAsync(new[] { Id });

        exitCode.Should().Be(-1);
    }
    
    [Fact]
    public async Task ReturnZeroCode_WhenArchiveInPendingStatus()
    {
        _messageHandlerMock.When(Url).Respond(HttpStatusCode.OK, JsonContent.Create(
            new
            {
                success = true,
                body = new {
                    status = "Pending"
                },
                errors = NullRef
            }
        ));

        var exitCode = await _command.InvokeAsync(new[] { Id });

        exitCode.Should().Be(0);
    }
    
    [Fact]
    public async Task ReturnProcessId_WhenArchiveInReadyStatus()
    {
        _messageHandlerMock.When(Url).Respond(HttpStatusCode.OK, JsonContent.Create(
            new
            {
                success = true,
                body = new {
                    status = "Completed"
                },
                errors = NullRef
            }
        ));

        var exitCode = await _command.InvokeAsync(new[] { Id });

        exitCode.Should().Be(int.Parse(Id));
    }
}