namespace AwesomeFiles.CLI.Tests;

public class AutoCheckingCommandShould
{
    private readonly Mock<IHttpClientFactory> _httpFactoryMock = new();
    private readonly MockHttpMessageHandler _messageHandlerMock = new();
    private readonly CreateArchiveCommand _command;
    
    private const string Id = "1";
    private const object NullRef = null!;
    
    private const string CreateArchiveUrl = "http://localhost:5001/process/start";
    private const string StatusUrl = $"http://localhost:5001/process/{Id}";
    private const string DownloadUrl = $"http://localhost:5001/process/download/{Id}";
    

    public AutoCheckingCommandShould()
    {
        _command = new CreateArchiveCommand(_httpFactoryMock.Object);
        
        _httpFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>()))
            .Returns(new HttpClient(_messageHandlerMock));
    }
    
    
}