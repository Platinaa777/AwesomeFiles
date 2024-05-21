using AwesomeFiles.Domain.ResultAbstractions;

namespace AwesomeFiles.Domain.Errors;

public class FileError : Error
{
    public static FileError FileNotExistsError(string file) =>
        new("FileError", $"File '{file}' does not exists in the system");
    
    public FileError(string code, string message) : base(code, message)
    {
    }
}