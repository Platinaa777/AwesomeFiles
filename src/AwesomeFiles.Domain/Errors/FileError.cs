using AwesomeFiles.Domain.ResultAbstractions;

namespace AwesomeFiles.Domain.Errors;

public class FileError : Error
{
    public static FileError AddFileNotExistsError(string file) =>
        new FileError("FileError", $"File '{file}' does not exists in the system");
    
    public FileError(string code, string message) : base(code, message)
    {
    }
}