using System.Collections.Concurrent;
using CSharpMicroManager.CQRS.Abstractions.Commands;

namespace CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.Handle;

public sealed class CommandHandlerPipelineContext<TCommand> where TCommand : ICommand
{
    public ConcurrentDictionary<string, object> ItemsBag { get; } = new();
    public TCommand Command { get; }

    public CommandHandlerPipelineContext(TCommand command)
    {
        Command = command;
    }
}