using CSharpMicroManager.CQRS.Abstractions.Commands;

namespace CSharpMicroManager.CQRS.Attributes.Command;

/// <summary>
/// Defines attribute which target is <see cref="ICommandHandler{TCommand}"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public abstract class CommandHandlerAttribute : Attribute
{
}