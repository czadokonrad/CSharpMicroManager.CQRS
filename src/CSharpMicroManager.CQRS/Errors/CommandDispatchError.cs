using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Errors;

public sealed record CommandDispatchError : Error
{
    public Exception Exception { get; }

    public CommandDispatchError(Type commandType, Exception exception) : 
        base($"Command: {commandType} failed with: {exception.Message}", null)
    {
        Exception = exception;
    }
}