namespace CSharpMicroManager.CQRS.Abstractions.Attributes.Command.Handler;
using Commands;

/// <summary>
/// Represents information about versions of <see cref="ICommand"/> allowed for specific <see cref="ICommandHandler{TCommand}"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class AllowCommandVersionAttribute : Attribute
{
    public IEnumerable<ushort> Versions { get; }

    public AllowCommandVersionAttribute(params ushort[] versions)
    {
        Versions = versions;
    }
}