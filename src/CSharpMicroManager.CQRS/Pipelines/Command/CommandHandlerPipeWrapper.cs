using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.Handle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PostHandle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PreHandle;
using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Pipelines.Command;

internal class CommandHandlerPipeWrapper<TCommand> 
    where TCommand : ICommand
{
    private readonly CommandPreHandlerPipelineDelegate<TCommand> _preHandler;
    private readonly CommandHandlerPipelineDelegate<TCommand> _handler;
    private readonly CommandPostHandlerPipelineDelegate<TCommand> _postHandler;

    public CommandHandlerPipeWrapper(
        CommandPreHandlerPipelineDelegate<TCommand> preHandler,
        CommandHandlerPipelineDelegate<TCommand> handler,
        CommandPostHandlerPipelineDelegate<TCommand> postHandler)
    {
        _preHandler = preHandler;
        _handler = handler;
        _postHandler = postHandler;
    }
    
    public async Task<Result<Unit>> Handle(TCommand command, CancellationToken cancellationToken)
    {
        var context = new CommandHandlerPipelineContext<TCommand>(command);
        var result = await _preHandler(new CommandPreHandlerPipelineContext<TCommand>(context.Command), cancellationToken);

        if (result.Errors.Any())
        {
            return result;
        }
        
        return await _handler(context, cancellationToken)
            .Bind(_ => _postHandler(new CommandPostHandlerPipelineContext<TCommand>(context.Command, result), cancellationToken));
    }
}