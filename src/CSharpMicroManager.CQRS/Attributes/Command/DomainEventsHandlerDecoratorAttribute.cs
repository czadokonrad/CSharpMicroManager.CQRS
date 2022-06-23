using CSharpMicroManager.CQRS.Decorators.Command;

namespace CSharpMicroManager.CQRS.Attributes.Command;

/// <summary>
/// Defines attribute which target is <see cref="CommandHandlerDecorator{TCommand}"/> responsible for dispatching of domain events
/// <remarks>This attribute is a helper which is used by DI registration extension for detecting instance of decorator and register it in correct order</remarks>
/// </summary>
public sealed class DomainEventsHandlerDecoratorAttribute : CommandHandlerAttribute
{
}