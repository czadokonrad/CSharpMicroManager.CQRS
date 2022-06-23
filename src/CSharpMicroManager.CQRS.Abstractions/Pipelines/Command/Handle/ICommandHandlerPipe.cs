using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.Handle;

/// <summary>
/// Represents a functionality of CommandHandlerPipe, which will be executed during processing actual <see cref="ICommandHandler{TCommand}"/> on <see cref="ICommand"/>
/// </summary>
/// <typeparam name="TCommand">Concrete type of <see cref="ICommand"/></typeparam>
public interface ICommandHandlerPipe<TCommand> where TCommand : ICommand
{
    /// <summary>
    /// Handles single step of registered CommandHandlerPipeline by using <see cref="ICommandHandlerPipelineBuilder{TCommand}"/>
    /// </summary>
    /// <param name="context">Contextual information</param>
    /// <param name="next">Next handler of type <see cref="ICommandHandlerPipe{TCommand}"/></param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns></returns>
    Task<Result<Unit>> Handle(
        CommandHandlerPipelineContext<TCommand> context,
        CommandHandlerPipelineDelegate<TCommand> next,
        CancellationToken cancellationToken);
}

/// <summary>
/// Delegate which represents a pipeline of registered <see cref="ICommandHandlerPipe{TCommand}"/>
/// </summary>
/// <typeparam name="TCommand">Concrete type of <see cref="ICommand"/></typeparam>
public delegate Task<Result<Unit>> CommandHandlerPipelineDelegate<TCommand>(
    CommandHandlerPipelineContext<TCommand> context,
    CancellationToken cancellationToken) where TCommand : ICommand;