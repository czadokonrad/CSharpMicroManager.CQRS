using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.Handle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PostHandle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PreHandle;
using Microsoft.Extensions.DependencyInjection;

namespace CSharpMicroManager.CQRS.Pipelines.Command;

[Obsolete]
internal sealed class CommandPipelineBuilder
{
    private readonly IServiceProvider _serviceProvider;

    public CommandPipelineBuilder(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public CommandPreHandlerPipelineDelegate<TCommand> ForPreHandler<TCommand>() where TCommand : ICommand
    {
        var preHandlerPipeLineBuilder = _serviceProvider.GetRequiredService<ICommandPreHandlerPipelineBuilder<TCommand>>();
        var preHandlerProviders = _serviceProvider.GetRequiredService<IEnumerable<ICommandPreHandlerPipe<TCommand>>>();

        return preHandlerProviders
            .Aggregate(preHandlerPipeLineBuilder, (builder, pipe) =>
            {
                return builder.UsePipe(next => { return (context, ct) => pipe.Handle(context, next, ct); });
            })
            .Build();
    }
    
    public CommandHandlerPipelineDelegate<TCommand> ForHandler<TCommand>() where TCommand : ICommand
    {
        var handlerPipeLineBuilder = _serviceProvider.GetRequiredService<ICommandHandlerPipelineBuilder<TCommand>>();
        var handlerProviders = _serviceProvider.GetRequiredService<IEnumerable<ICommandHandlerPipe<TCommand>>>();
       
        return handlerProviders
            .Aggregate(handlerPipeLineBuilder, (builder, pipe) =>
            {
                return builder.UsePipe(next => { return (context, ct) => pipe.Handle(context, next, ct); });
            })
            .Build();
    }
    
    public CommandPostHandlerPipelineDelegate<TCommand> ForPostHandler<TCommand>() where TCommand : ICommand
    {
        var postHandlerPipeLineBuilder = _serviceProvider.GetRequiredService<ICommandPostHandlerPipelineBuilder<TCommand>>();
        var postHandlerProviders = _serviceProvider.GetRequiredService<IEnumerable<ICommandPostHandlerPipe<TCommand>>>();
      
        return postHandlerProviders
            .Aggregate(postHandlerPipeLineBuilder, (builder, pipe) =>
            {
                return builder.UsePipe(next => { return (context, ct) => pipe.Handle(context, next, ct); });
            })
            .Build();
    }
}