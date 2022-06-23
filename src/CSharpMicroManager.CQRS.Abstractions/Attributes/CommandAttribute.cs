using CSharpMicroManager.CQRS.Abstractions.Commands;

namespace CSharpMicroManager.CQRS.Abstractions.Attributes;

/// <summary>
/// Base attribute for attributes which are dedicated for <see cref="ICommand"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public abstract class CommandAttribute : Attribute
{
    public abstract string Description { get; }
}

