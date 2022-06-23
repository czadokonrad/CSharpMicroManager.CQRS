using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PreHandle;

namespace CSharpMicroManager.CQRS.Pipelines.Command.Cache;

internal readonly struct CachedPreHandlerPipeline<TCommand> where TCommand : ICommand
{
    public CommandPreHandlerPipelineDelegate<TCommand> Pipeline { get; }

    public CachedPreHandlerPipeline(CommandPreHandlerPipelineDelegate<TCommand> pipeline)
    {
        Pipeline = pipeline;
    }
}