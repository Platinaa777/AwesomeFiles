namespace AwesomeFiles.Domain.ResultAbstractions;

public class Result
{
    protected Result(bool isSuccess, params Error[] errors)
    {
        if (isSuccess && errors.Any(x => x.Code != Error.None) ||
            !isSuccess && errors.Any(x => x.Code == Error.None))
        {
            throw new ArgumentException("Invalid result value");
        }
        IsSuccess = isSuccess;
        Errors = errors.ToList();
    }
    
    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public List<Error> Errors { get; }

    public static Result Success() => new(true, Error.None);

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Error.None);

    public static Result Failure(params Error[] errors) => new(false, errors);

    public static Result<TValue> Failure<TValue>(params Error[] errors) => new(default, false, errors);

    public static Result Create(bool condition) => condition ? Success() : Failure(Error.UnforseenFail);

    public static Result<TValue> Create<TValue>(TValue? value) => value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
}