using CSharpMicroManager.Functional.Core;
using FluentValidation.Results;

namespace CSharpMicroManager.CQRS.Errors.Business;

public sealed record CommandValidationError : Error
{
    private readonly List<CommandFieldError> _errors = new();

    public CommandValidationError(IEnumerable<ValidationFailure> failures) : base(
        "Command validation error(s) occured", null)
    {
        _errors = failures.Select(f => new CommandFieldError(f.PropertyName, f.ErrorMessage, f.ErrorCode))
            .ToList();
    }

    public IReadOnlyList<CommandFieldError> Errors => _errors.AsReadOnly();
}
        
public sealed record CommandFieldError(string PropertyName, string Message, string? ErrorCode)
{
}