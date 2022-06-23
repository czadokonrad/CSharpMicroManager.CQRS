using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.Handle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PostHandle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PreHandle;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CSharpMicroManager.CQRS.Pipelines.Command;

internal sealed class CommandPipelineBuilderFactory
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILoggerFactory _loggerFactory;

    public CommandPipelineBuilderFactory(
        IServiceProvider serviceProvider,
        ILoggerFactory loggerFactory)
    {
        _serviceProvider = serviceProvider;
        _loggerFactory = loggerFactory;
    }

    public CommandHandlerPipeWrapper<TCommand> CreatePipeline<TCommand>()
        where TCommand : ICommand
    {
        return new CommandHandlerPipeWrapper<TCommand>(
            GetCommandPreHandler<TCommand>(_serviceProvider),
            GetCommandHandler<TCommand>(_serviceProvider),
            GetCommandPostHandler<TCommand>(_serviceProvider),
            _loggerFactory.CreateLogger<CommandHandlerPipeWrapper<TCommand>>());
    }
    
    private CommandPreHandlerPipelineDelegate<TCommand> GetCommandPreHandler<TCommand>(IServiceProvider serviceProvider) where TCommand : ICommand
    {
        ICommandPreHandlerPipelineBuilder<TCommand> preHandlerPipeLineBuilder = new CommandPreHandlerPipelineBuilder<TCommand>();
        var preHandlerProviders = serviceProvider.GetRequiredService<IEnumerable<ICommandPreHandlerPipe<TCommand>>>();
        
        return preHandlerProviders
            .Aggregate(preHandlerPipeLineBuilder, (builder, pipe) =>
            {
                return builder.UsePipe(next => { return (context, ct) => pipe.Handle(context, next, ct); });
            })
            .Build();
    }
    
    private CommandHandlerPipelineDelegate<TCommand> GetCommandHandler<TCommand>(IServiceProvider serviceProvider) where TCommand : ICommand
    {
        ICommandHandlerPipelineBuilder<TCommand> handlerPipeLineBuilder = new CommandHandlerPipelineBuilder<TCommand>();
        var handlerProviders = serviceProvider.GetRequiredService<IEnumerable<ICommandHandlerPipe<TCommand>>>();
        
        return handlerProviders
            .Aggregate(handlerPipeLineBuilder, (builder, pipe) =>
            {
                return builder.UsePipe(next => { return (context, ct) => pipe.Handle(context, next, ct); });
            })
            .Build();
    }
    
    private CommandPostHandlerPipelineDelegate<TCommand> GetCommandPostHandler<TCommand>(IServiceProvider serviceProvider) where TCommand : ICommand
    {
        ICommandPostHandlerPipelineBuilder<TCommand> postHandlerPipeLineBuilder = new CommandPostHandlerPipelineBuilder<TCommand>();
        var postHandlerProviders = serviceProvider.GetRequiredService<IEnumerable<ICommandPostHandlerPipe<TCommand>>>();
        
        return postHandlerProviders
            .Aggregate(postHandlerPipeLineBuilder, (builder, pipe) =>
            {
                return builder.UsePipe(next => { return (context, ct) => pipe.Handle(context, next, ct); });
            })
            .Build();
    }
}
