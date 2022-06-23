using CSharpMicroManager.CQRS.Abstractions.Commands;

namespace CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PostHandle;

/// <summary>
/// Builds a pipeline used for preprocessing <see cref="ICommand"/> for <see cref="ICommandHandler{TCommand}"/>
/// <para></para>
/// Which executes all pipes after <see cref="ICommandHandler{TCommand}.Handle(TCommand, CancellationToken)"/> is executed
/// </summary>
public interface ICommandPostHandlerPipelineBuilder<TCommand> where TCommand : ICommand
{
    ICommandPostHandlerPipelineBuilder<TCommand> UsePipe(Func<CommandPostHandlerPipelineDelegate<TCommand>, CommandPostHandlerPipelineDelegate<TCommand>> middleware);
    ICommandPostHandlerPipelineBuilder<TCommand> UsePipe(ICommandPostHandlerPipe<TCommand> pipe);
    ICommandPostHandlerPipelineBuilder<TCommand> UsePipe<TCommandPostHandlerPipe>() where 
        TCommandPostHandlerPipe : class, ICommandPostHandlerPipe<TCommand>, new();
    CommandPostHandlerPipelineDelegate<TCommand> Build();
    IReadOnlyCollection<Func<CommandPostHandlerPipelineDelegate<TCommand>, CommandPostHandlerPipelineDelegate<TCommand>>> Pipes { get; }
}