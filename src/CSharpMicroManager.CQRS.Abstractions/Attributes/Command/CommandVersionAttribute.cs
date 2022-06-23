namespace CSharpMicroManager.CQRS.Abstractions.Attributes.Command;

/// <summary>
/// Specifies version of command
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class CommandVersionAttribute : Attribute
{
    public ushort Version { get; }

    public CommandVersionAttribute(ushort version)
    {
        Version = version;
    }
}