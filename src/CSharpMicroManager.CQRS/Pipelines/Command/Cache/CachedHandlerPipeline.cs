using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.Handle;

namespace CSharpMicroManager.CQRS.Pipelines.Command.Cache;

internal readonly struct CachedHandlerPipeline<TCommand>  where TCommand : ICommand
{
    public CommandHandlerPipelineDelegate<TCommand> Pipeline { get; }

    public CachedHandlerPipeline(CommandHandlerPipelineDelegate<TCommand> pipeline)
    {
        Pipeline = pipeline;
    }
}