namespace AwesomeFiles.HttpModels.Responses;

public class ApiResponse<T> where T : class
{
    public static ApiResponse<T> ReturnSuccess(T body) => new(true, body, null);

    public static ApiResponse<T> ReturnFailure(List<string> errors) => new(false, null, errors);
    
    public ApiResponse(bool success, T? body, List<string>? errors)
    {
        Success = success;
        Body = body;
        Errors = errors;
    }
    
    public bool Success { get; set; }
    public T? Body { get; set; }
    public List<string>? Errors { get; set; }
}