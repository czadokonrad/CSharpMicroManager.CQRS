using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.Handle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PostHandle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PreHandle;

namespace CSharpMicroManager.CQRS.Pipelines.Command.Cache;

internal sealed class CommandHandlerPipelinesCache<TCommand> where TCommand : ICommand
{
    private readonly Dictionary<Type, CommandPreHandlerPipelineDelegate<TCommand>> _preCommandHandlerPipelines = new();
    private readonly Dictionary<Type, CommandHandlerPipelineDelegate<TCommand>> _commandHandlerPipelines = new();
    private readonly Dictionary<Type, CommandPostHandlerPipelineDelegate<TCommand>> _postCommandHandlerPipelines = new();

    public CommandPreHandlerPipelineDelegate<TCommand>? GetPreHandlePipeline()
    {
        if(_preCommandHandlerPipelines.TryGetValue(typeof(TCommand), out var @delegate))
        {
            return @delegate;
        }

        return null;
    }
    
    public CommandHandlerPipelineDelegate<TCommand>? GetHandlerPipeline()
    {
        if(_commandHandlerPipelines.TryGetValue(typeof(TCommand), out var @delegate))
        {
            return @delegate;
        }
        
        return null;
    }
    
    public CommandPostHandlerPipelineDelegate<TCommand>? GetPostHandlerPipeline()
    {
        if(_postCommandHandlerPipelines.TryGetValue(typeof(TCommand), out var @delegate))
        {
            return @delegate;
        }
        
        return null;
    }

    internal void SetPreHandlerPipeline(CommandPreHandlerPipelineDelegate<TCommand> pipelineDelegate) =>
        _preCommandHandlerPipelines[typeof(TCommand)] = pipelineDelegate;
    
    internal void SetHandlerPipeline(CommandHandlerPipelineDelegate<TCommand> pipelineDelegate) =>
        _commandHandlerPipelines[typeof(TCommand)] = pipelineDelegate;
    
    internal void SetPostHandlerPipeline(CommandPostHandlerPipelineDelegate<TCommand> pipelineDelegate) =>
        _postCommandHandlerPipelines[typeof(TCommand)] = pipelineDelegate;
}