using System.Collections.Concurrent;

namespace CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PreHandle;

public sealed class CommandPreHandlerPipelineContext<TCommand> 
{
    public ConcurrentDictionary<string, object> ItemsBag { get; } = new();
    public TCommand Command { get; }

    public CommandPreHandlerPipelineContext(TCommand command)
    {
        Command = command;
    }
}