using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.Handle;
using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Pipelines.Command;

/// <summary>
/// Pipe which is responsible for handling actual <see cref="TCommand"/> by using <see cref="ICommandHandler{TCommand}"/>
/// </summary>
/// <typeparam name="TCommand"></typeparam>
internal sealed class CommandHandlingPipe<TCommand> : ICommandHandlerPipe<TCommand>
where TCommand : ICommand
{
    private readonly ICommandHandler<TCommand> _commandHandler;

    public CommandHandlingPipe(ICommandHandler<TCommand> commandHandler)
    {
        _commandHandler = commandHandler;
    }
    public async Task<Result<Unit>> Handle(
        TCommand command, 
        CommandHandlerPipelineDelegate<TCommand> next,
        CancellationToken cancellationToken)
    {
        var commandResult = await _commandHandler.Handle(command, cancellationToken);

        if (!commandResult.IsError)
        {
            return await next(command, cancellationToken);
        }

        return commandResult;
    }
}