using CSharpMicroManager.CQRS.Abstractions.Commands;

namespace CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.Handle;

public interface ICommandHandlerPipelineBuilder<TCommand> where TCommand : ICommand
{
    ICommandHandlerPipelineBuilder<TCommand> UsePipe(
        Func<CommandHandlerPipelineDelegate<TCommand>, CommandHandlerPipelineDelegate<TCommand>> middleware);

    ICommandHandlerPipelineBuilder<TCommand> UsePipe(ICommandHandlerPipe<TCommand> pipe);

    ICommandHandlerPipelineBuilder<TCommand> UsePipe<TCommandHandlerPipe>()
        where TCommandHandlerPipe : class, ICommandHandlerPipe<TCommand>, new();

    CommandHandlerPipelineDelegate<TCommand> Build(IEnumerable<ICommandHandlerPipe<TCommand>> pipes);
    CommandHandlerPipelineDelegate<TCommand> Build();

    IReadOnlyCollection<Func<CommandHandlerPipelineDelegate<TCommand>, CommandHandlerPipelineDelegate<TCommand>>> Pipes
    {
        get;
    }
}