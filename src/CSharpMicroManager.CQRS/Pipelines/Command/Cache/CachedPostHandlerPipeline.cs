using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PostHandle;

namespace CSharpMicroManager.CQRS.Pipelines.Command.Cache;

internal readonly struct CachedPostHandlerPipeline<TCommand>  where TCommand : ICommand
{
    public CommandPostHandlerPipelineDelegate<TCommand> Pipeline { get; }

    public CachedPostHandlerPipeline(CommandPostHandlerPipelineDelegate<TCommand> pipeline)
    {
        Pipeline = pipeline;
    }
}