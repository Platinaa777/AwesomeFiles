using System.CommandLine;
using System.Net;
using System.Net.Http.Json;
using af.Services;

namespace AwesomeFiles.CLI.Tests;

public class DownloadArchiveCommandShould
{
    private readonly Mock<IHttpClientFactory> _httpFactoryMock = new();
    private readonly Mock<IFileService> _fileServiceMock = new();
    private readonly MockHttpMessageHandler _messageHandlerMock = new();
    private readonly DownloadCommand _command;
    private const string Id = "1";
    private const string Url = $"http://localhost:5001/process/download/{Id}";
    
    public DownloadArchiveCommandShould()
    {
        _command = new DownloadCommand(_httpFactoryMock.Object, _fileServiceMock.Object);
        _httpFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(_messageHandlerMock));
        _fileServiceMock.Setup(x => x.WriteAllBytesAsync(It.IsAny<string>(), It.IsAny<byte[]>()))
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task ReturnNegativeStatusCode_WhenHttpResponseFailure()
    {
        _messageHandlerMock.When(Url).Respond(HttpStatusCode.BadRequest, JsonContent.Create(new {
            errorMessage = $"Архив с id: {Id} не был найден"
        }));
        
        var exitCode = await _command.InvokeAsync(new[] { Id, "PATH" });

        exitCode.Should().Be(-1);
    }
    
    [Fact]
    public async Task ReturnProcessId_WhenProcessIsReady()
    {
        var path = "PATH";
        
        _messageHandlerMock.When(Url).Respond(HttpStatusCode.OK, JsonContent.Create(
            new ByteArrayContent(new byte[] { 1, 2, 3, 4 })
        ));

        var exitCode = await _command.InvokeAsync(new[] { Id, path });

        exitCode.Should().Be(int.Parse(Id));
        _fileServiceMock.Verify(x => x.WriteAllBytesAsync(Path.Combine(path, $"archive-{Id}.zip"), It.IsAny<byte[]>()), Times.Once);
    }
}