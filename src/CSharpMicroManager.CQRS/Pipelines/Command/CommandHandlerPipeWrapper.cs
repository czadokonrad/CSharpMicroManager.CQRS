using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.Handle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PostHandle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PreHandle;
using CSharpMicroManager.Functional.Core;
using Microsoft.Extensions.Logging;

namespace CSharpMicroManager.CQRS.Pipelines.Command;

internal class CommandHandlerPipeWrapper<TCommand>
    where TCommand : ICommand
{
    private readonly CommandPreHandlerPipelineDelegate<TCommand> _preHandler;
    private readonly CommandHandlerPipelineDelegate<TCommand> _handler;
    private readonly CommandPostHandlerPipelineDelegate<TCommand> _postHandler;
    private readonly ILogger<CommandHandlerPipeWrapper<TCommand>> _logger;

    public CommandHandlerPipeWrapper(
        CommandPreHandlerPipelineDelegate<TCommand> preHandler,
        CommandHandlerPipelineDelegate<TCommand> handler,
        CommandPostHandlerPipelineDelegate<TCommand> postHandler,
        ILogger<CommandHandlerPipeWrapper<TCommand>> logger)
    {
        _preHandler = preHandler;
        _handler = handler;
        _postHandler = postHandler;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var context = new CommandHandlerPipelineContext<TCommand>(command);
        var result = await _preHandler(new CommandPreHandlerPipelineContext<TCommand>(context.Command),
            cancellationToken);

        if (result.Errors.Any())
        {
            return result;
        }

        var handlerResult = await _handler(context, cancellationToken);

        try
        {
            var postHandlerResult = await _postHandler(new CommandPostHandlerPipelineContext<TCommand>(context.Command, result), cancellationToken);

            if (postHandlerResult.IsError)
            {
                _logger.LogError("Executing of CommandPostHandlerPipeline for command: {Command} finished with following errors: {Errors}",
                    typeof(TCommand).FullName,
                    string.Join(Environment.NewLine, postHandlerResult.Errors.Select(e => e.Message)));
            }
        }
        catch(Exception ex)
        {
            _logger.LogCritical(ex, "Unhandled error during executing CommandPostHandlerPipeline for command: {Command}", typeof(TCommand).FullName);
        }

        return handlerResult;
    }
}