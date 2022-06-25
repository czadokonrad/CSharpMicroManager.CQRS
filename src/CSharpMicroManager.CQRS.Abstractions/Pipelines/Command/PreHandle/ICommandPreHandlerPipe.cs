using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PreHandle
{
    /// <summary>
    /// Represents a functionality of PreCommandHandlerPipe, which will be executed before actual <see cref="ICommandHandler{TCommand}"/> on <see cref="ICommand"/>
    /// </summary>
    /// <typeparam name="TCommand">Concrete type of <see cref="ICommand"/></typeparam>
    public interface ICommandPreHandlerPipe<TCommand> where TCommand : ICommand
    {
        /// <summary>
        /// Handles single step of registered PreCommandHandlerPipeline by using <see cref="ICommandPreHandlerPipelineBuilder{TCommand}"/>
        /// </summary>
        /// <param name="command">Command to process</param>
        /// <param name="next">Next handler of type <see cref="ICommandPreHandlerPipe{TCommand}"/></param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        Task<Result<Unit>> Handle(
            TCommand command,
            CommandPreHandlerPipelineDelegate<TCommand> next, 
            CancellationToken cancellationToken);
    }
    

    /// <summary>
    /// Delegate which represents a pipeline of registered <see cref="ICommandPreHandlerPipe{TCommand}"/>
    /// </summary>
    /// <typeparam name="TCommand">Concrete type of <see cref="ICommand"/></typeparam>
    public delegate Task<Result<Unit>> CommandPreHandlerPipelineDelegate<TCommand>(
        TCommand command, 
        CancellationToken cancellationToken) where TCommand : ICommand;
}