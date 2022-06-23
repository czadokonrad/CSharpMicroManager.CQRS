using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Dispatching.Command;
using CSharpMicroManager.CQRS.Errors;
using CSharpMicroManager.Functional.Core;
using Microsoft.Extensions.Logging;

namespace CSharpMicroManager.CQRS.Dispatching.Command;

internal sealed class CommandDispatcherWithErrorHandlingDecorator : ICommandDispatcher
{
    private readonly ICommandDispatcher _decorated;
    private readonly ILogger<CommandDispatcherWithErrorHandlingDecorator> _logger;

    public CommandDispatcherWithErrorHandlingDecorator(
        ICommandDispatcher decorated,
        ILogger<CommandDispatcherWithErrorHandlingDecorator> logger)
    {
        _decorated = decorated;
        _logger = logger;
    }
    
    public Task<Result<Unit>> Handle<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : ICommand
    {
        try
        {
            return _decorated.Handle(command, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "During processing: {Command} occurred exception: {Message}", typeof(TCommand), e.Message);
            return Task.FromResult(new Result<Unit>(new CommandDispatchError(typeof(TCommand), e)));
        }
    }
}