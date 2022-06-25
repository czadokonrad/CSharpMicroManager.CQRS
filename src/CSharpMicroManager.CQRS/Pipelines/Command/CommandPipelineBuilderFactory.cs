using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.Handle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PostHandle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PreHandle;
using Microsoft.Extensions.Logging;

namespace CSharpMicroManager.CQRS.Pipelines.Command;

internal sealed class CommandPipelineBuilderFactory<TCommand> where TCommand : ICommand
{
    private readonly ICommandPreHandlerPipelineBuilder<TCommand> _preHandlerPipeLineBuilder;
    private readonly ICommandHandlerPipelineBuilder<TCommand> _handlerPipeLineBuilder;
    private readonly ICommandPostHandlerPipelineBuilder<TCommand> _postHandlerPipeLineBuilder;
    private readonly IEnumerable<ICommandPreHandlerPipe<TCommand>> _preHandlerPipes;
    private readonly IEnumerable<ICommandHandlerPipe<TCommand>> _handlerPipes;
    private readonly IEnumerable<ICommandPostHandlerPipe<TCommand>> _postHandlerPipes;
    private readonly ILoggerFactory _loggerFactory;

    public CommandPipelineBuilderFactory(
        ICommandPreHandlerPipelineBuilder<TCommand> preHandlerPipeLineBuilder,
        ICommandHandlerPipelineBuilder<TCommand> handlerPipeLineBuilder,
        ICommandPostHandlerPipelineBuilder<TCommand> postHandlerPipeLineBuilder,
        IEnumerable<ICommandPreHandlerPipe<TCommand>> preHandlerPipes,
        IEnumerable<ICommandHandlerPipe<TCommand>> handlerPipes,
        IEnumerable<ICommandPostHandlerPipe<TCommand>> postHandlerPipes,
        ILoggerFactory loggerFactory)
    {
        _preHandlerPipeLineBuilder = preHandlerPipeLineBuilder;
        _handlerPipeLineBuilder = handlerPipeLineBuilder;
        _postHandlerPipeLineBuilder = postHandlerPipeLineBuilder;
        _preHandlerPipes = preHandlerPipes;
        _handlerPipes = handlerPipes;
        _postHandlerPipes = postHandlerPipes;
        _loggerFactory = loggerFactory;
    }

    public CommandHandlerPipeWrapper<TCommand> CreatePipeline()
    {
        return new CommandHandlerPipeWrapper<TCommand>(
            _preHandlerPipeLineBuilder.Build(_preHandlerPipes),
            _handlerPipeLineBuilder.Build(_handlerPipes),
            _postHandlerPipeLineBuilder.Build(_postHandlerPipes),
            _loggerFactory.CreateLogger<CommandHandlerPipeWrapper<TCommand>>());
    }
}
