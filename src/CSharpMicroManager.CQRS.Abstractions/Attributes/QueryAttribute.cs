using CSharpMicroManager.CQRS.Abstractions.Queries;

namespace CSharpMicroManager.CQRS.Abstractions.Attributes;

/// <summary>
/// Base attribute for attributes which are dedicated for <see cref="IQuery{TResult}"/>
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public abstract class QueryAttribute : Attribute
{
    public abstract string Description { get; }
}