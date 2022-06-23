namespace CSharpMicroManager.CQRS.Abstractions.Attributes.Command;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, AllowMultiple = true)]
public class RequirePermission<TCommandPermission> : Attribute where TCommandPermission : CommandPermission
{
}

/// <summary>
/// Represents permission
/// </summary>
public abstract record CommandPermission
{
    /// <summary>
    /// Unique identifier of <see cref="Permission"/>
    /// </summary>
    public abstract Guid Id { get; }

    /// <summary>
    /// Name of the <see cref="Permission"/>. Defaults to the name of the type.
    /// </summary>
    public virtual string Name => GetType().Name;
}