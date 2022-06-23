namespace CSharpMicroManager.CQRS.Abstractions.Commands;

/// <summary>
/// Defines a command, that creates a new resource
/// </summary>
public interface ICreationCommand : ICommand
{
    /// <summary>
    /// Identifier of created resource
    /// </summary>
    Guid CreatedId { get; }
}