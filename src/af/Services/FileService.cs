namespace af.Services;

public interface IFileService
{
    Task WriteAllBytesAsync(string path, byte[] bytes);
}

public class FileService : IFileService
{
    public async Task WriteAllBytesAsync(string path, byte[] bytes)
    {
        await File.WriteAllBytesAsync(path, bytes);
    }
}
