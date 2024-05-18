namespace AwesomeFiles.Domain.ResultAbstractions;

public class Error : IEquatable<Error>
{
    public static readonly Error None = new Error(String.Empty, String.Empty);
    public static readonly Error UnforseenFail = new("Error.UnforseenFail", "UnforseenFail");
    public static readonly Error NullValue = new("Error.NullValue", "The specified result value is null");
    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public string Message { get; }
    public string Code { get; }
    public static implicit operator string(Error error) => error.Code;

    public bool Equals(Error? other) =>
        other is not null && other.Message.Equals(Message) && other.Code.Equals(Code);

    public static bool operator ==(Error? left, Error? right)
    {
        if (left is null && right is null) return true;
        if (left is null || right is null) return false;

        return left.Equals(right);
    }

    public static bool operator !=(Error? left, Error? right)
    {
        return !(left == right);
    }

    public override bool Equals(object? obj) =>
        obj is Error failure && Equals(failure);

    public override int GetHashCode()
    {
        return HashCode.Combine(Message, Code);
    }
}