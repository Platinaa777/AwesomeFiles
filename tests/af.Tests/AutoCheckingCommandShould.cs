using System.CommandLine;
using System.Net;
using System.Net.Http.Json;
using af.Services;

namespace AwesomeFiles.CLI.Tests;

public class AutoCheckingCommandShould
{
    private readonly Mock<IHttpClientFactory> _httpFactoryMock = new();
    private readonly Mock<IFileService> _fileServiceMock = new();
    private readonly MockHttpMessageHandler _messageHandlerMock = new();
    private readonly AutoCheckingCommand _command;
    
    private const string Id = "1";
    private const object NullRef = null!;
    private const string Path = "PATH";
    
    private const string CreateArchiveUrl = "http://localhost:5001/process/start";
    private const string StatusUrl = $"http://localhost:5001/process/{Id}";
    private const string DownloadUrl = $"http://localhost:5001/process/download/{Id}";

    public AutoCheckingCommandShould()
    {
        _command = new AutoCheckingCommand(_httpFactoryMock.Object, _fileServiceMock.Object);
        
        _httpFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(() => new HttpClient(_messageHandlerMock));
        _fileServiceMock.Setup(x => x.WriteAllBytesAsync(It.IsAny<string>(), It.IsAny<byte[]>()))
            .Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task ReturnNegativeExitCode_WhenBackend_Returns_ArchiveProcessCantBeStart()
    {
        _messageHandlerMock.When(CreateArchiveUrl).Respond(HttpStatusCode.BadRequest, JsonContent.Create(new {
            success = false,
            body = NullRef,
            errors = new[] {
                "File 'file3' does not exists in the system"
            }
        }));

        var exitCode = await _command.InvokeAsync(new[] { Path, "file1", "file2" });

        exitCode.Should().Be(-1);
    }
    
    [Fact]
    public async Task ReturnNegativeExitCode_WhenStatusCommandReturnedNegativeCode()
    {
        _messageHandlerMock.When(CreateArchiveUrl).Respond(HttpStatusCode.OK, JsonContent.Create(new {
            success = true,
            body =  new {
                id = 1
            },
            errors = NullRef
        }));
        
        _messageHandlerMock.When(StatusUrl).Respond(HttpStatusCode.BadRequest, JsonContent.Create(
            new { smth = "empty" }
        ));

        var exitCode = await _command.InvokeAsync(new[] { Path, "file1", "file2" });

        exitCode.Should().Be(-1);
    }
    
    [Fact]
    public async Task ReturnNegativeExitCode_WhenBackend_DownloadCommandCantInstallZipFile()
    {
        _messageHandlerMock.When(CreateArchiveUrl).Respond(HttpStatusCode.OK, JsonContent.Create(new {
            success = true,
            body =  new {
                id = 1
            },
            errors = NullRef
        }));
        
        _messageHandlerMock.When(StatusUrl).Respond(HttpStatusCode.OK, JsonContent.Create(
            new
            {
                success = true,
                body = new {
                    status = "Completed"
                },
                errors = NullRef
            }
        ));
        
        _messageHandlerMock.When(DownloadUrl).Respond(HttpStatusCode.BadRequest, JsonContent.Create(new {
            errorMessage = "Server errors"
        }));

        var exitCode = await _command.InvokeAsync(new[] { Path, "file1", "file2" });

        exitCode.Should().Be(-1);
    }
    
    [Fact]
    public async Task ReturnProcessId_WhenBackend_SuccessfullyHandledArchiveProcessing()
    {
        _messageHandlerMock.When(CreateArchiveUrl).Respond(HttpStatusCode.OK, JsonContent.Create(new {
            success = true,
            body =  new {
                id = 1
            },
            errors = NullRef
        }));
        
        _messageHandlerMock.When(StatusUrl).Respond(HttpStatusCode.OK, JsonContent.Create(
            new
            {
                success = true,
                body = new {
                    status = "Completed"
                },
                errors = NullRef
            }
        ));
        
        _messageHandlerMock.When(DownloadUrl).Respond(HttpStatusCode.OK, JsonContent.Create(
            new ByteArrayContent(new byte[] { 1, 2, 3, 4 })
        ));

        var exitCode = await _command.InvokeAsync(new[] { Path, "file1", "file2" });

        exitCode.Should().Be(int.Parse(Id));
    }
}