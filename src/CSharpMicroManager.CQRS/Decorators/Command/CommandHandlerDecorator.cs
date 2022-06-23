using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Decorators.Command;

/// <summary>
/// Marker type for decorators for <see cref="ICommandHandler{TCommand}"/>
/// </summary>
/// <typeparam name="TCommand">Concrete type of <see cref="ICommand"/></typeparam>
public abstract class CommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    where TCommand : ICommand
{
    /// <summary>
    /// Processes <see cref="ICommand"/> adding some behavior and executes underlying decorated <see cref="ICommandHandler{TCommand}"/>
    /// </summary>
    /// <param name="command">Concrete type of <see cref="ICommand"/></param>
    /// <param name="cancellationToken">CancellationToken</param>
    /// <returns></returns>
    public abstract Task<Result<Unit>> Handle(TCommand command, CancellationToken cancellationToken);
}