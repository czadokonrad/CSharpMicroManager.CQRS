using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PostHandle
{
    /// <summary>
    /// Represents a functionality of PostCommandHandlerPipe, which will be executed after actual <see cref="ICommandHandler{TCommand}"/> on <see cref="ICommand"/>
    /// </summary>
    /// <typeparam name="TCommand">Concrete type of <see cref="ICommand"/></typeparam>
    public interface ICommandPostHandlerPipe<TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Handles single step of registered PostCommandHandlerPipeline by using <see cref="ICommandPostHandlerPipelineBuilder{TCommand}"/>
        /// </summary>
        /// <param name="context">Contextual information</param>
        /// <param name="next">Next handler of type <see cref="ICommandPostHandlerPipe{TCommand}"/></param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task<Result<Unit>> Handle(
          CommandPostHandlerPipelineContext<TCommand> context,
          CommandPostHandlerPipelineDelegate<TCommand> next,
          CancellationToken cancellationToken);
    }

    /// <summary>
    /// Delegate which represents a pipeline of registered <see cref="ICommandPostHandlerPipe{TCommand}"/>
    /// </summary>
    /// <typeparam name="TCommand">Concrete type of <see cref="ICommand"/></typeparam>
    public delegate Task<Result<Unit>> CommandPostHandlerPipelineDelegate<TCommand>(
        CommandPostHandlerPipelineContext<TCommand> context,
        CancellationToken cancellationToken) where TCommand : ICommand;
}