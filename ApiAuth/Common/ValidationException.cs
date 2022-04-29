namespace ApiAuth.Common;

public class ValidationException: Exception
{
    public ValidationException(string message, IReadOnlyDictionary<string, string[]> errors) : base(message) =>
        Errors = errors;

    public IReadOnlyDictionary<string, string[]> Errors { get; }
}