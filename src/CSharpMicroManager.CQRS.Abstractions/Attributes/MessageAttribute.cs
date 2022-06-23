using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Queries;

namespace CSharpMicroManager.CQRS.Abstractions.Attributes;

/// <summary>
/// Base attribute for attributes which are dedicated for both <see cref="ICommand"/> and <see cref="IQuery{TResult}"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public abstract class MessageAttribute : Attribute
{
    public abstract string Description { get; }
}