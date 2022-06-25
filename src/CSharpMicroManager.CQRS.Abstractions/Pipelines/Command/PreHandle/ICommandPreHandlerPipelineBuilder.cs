using CSharpMicroManager.CQRS.Abstractions.Commands;

namespace CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PreHandle;

/// <summary>
/// Builds a pipeline used for preprocessing <see cref="ICommand"/> for <see cref="ICommandHandler{TCommand}"/>
/// <para></para>
/// Which executes all pipes before <see cref="ICommandHandler{TCommand}.Handle(TCommand, CancellationToken)"/> is executed
/// </summary>
public interface ICommandPreHandlerPipelineBuilder<TCommand> where TCommand : ICommand
{
    ICommandPreHandlerPipelineBuilder<TCommand> UsePipe(Func<CommandPreHandlerPipelineDelegate<TCommand>, CommandPreHandlerPipelineDelegate<TCommand>> middleware);
    ICommandPreHandlerPipelineBuilder<TCommand> UsePipe(ICommandPreHandlerPipe<TCommand> pipe);
    ICommandPreHandlerPipelineBuilder<TCommand> UsePipe<TCommandPreHandlerPipe>() 
        where TCommandPreHandlerPipe : class, ICommandPreHandlerPipe<TCommand>, new();
    
    CommandPreHandlerPipelineDelegate<TCommand> Build(IEnumerable<ICommandPreHandlerPipe<TCommand>> pipes);
    CommandPreHandlerPipelineDelegate<TCommand> Build();
    IReadOnlyCollection<Func<CommandPreHandlerPipelineDelegate<TCommand>, CommandPreHandlerPipelineDelegate<TCommand>>> Pipes { get; }

}