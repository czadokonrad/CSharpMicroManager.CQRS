using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Abstractions.Dispatching.Command;

/// <summary>
/// Represents mechanics for dispatching <see cref="ICommand"/>
/// </summary>
public interface ICommandDispatcher
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="command">Command which will be handled</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <typeparam name="TCommand">Concrete type of <see cref="ICommand"/></typeparam>
    /// <returns></returns>
    Task<Result<Unit>> Handle<TCommand>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand;
}