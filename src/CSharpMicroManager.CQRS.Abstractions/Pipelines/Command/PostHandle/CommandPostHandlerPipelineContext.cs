using System.Collections.Concurrent;
using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PostHandle;

public readonly struct CommandPostHandlerPipelineContext<TCommand>
{
    public ConcurrentDictionary<string, object> ItemsBag { get; } = new();
    public TCommand Command { get; }
    public Result<Unit> CommandHandlerResult { get; }
    public CommandPostHandlerPipelineContext(TCommand command, Result<Unit> commandHandlerResult)
    {
        Command = command;
        CommandHandlerResult = commandHandlerResult;
    }
}