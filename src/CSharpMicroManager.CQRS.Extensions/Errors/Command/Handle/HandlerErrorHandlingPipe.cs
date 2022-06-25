using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.Handle;
using CSharpMicroManager.CQRS.Pipelines.Command.Descriptors;
using CSharpMicroManager.Functional.Core;
using Microsoft.Extensions.Logging;

namespace CSharpMicroManager.CQRS.Extensions.Errors.Command.Handle;

internal sealed class HandlerErrorHandlingPipe<TCommand> : ICommandHandlerPipe<TCommand>
    where TCommand : ICommand
{
    private readonly ILogger<HandlerErrorHandlingPipe<TCommand>> _logger;

    public HandlerErrorHandlingPipe(ILogger<HandlerErrorHandlingPipe<TCommand>> logger)
    {
        _logger = logger;
    }
    public async Task<Result<Unit>> Handle(
        TCommand command, 
        CommandHandlerPipelineDelegate<TCommand> next,
        CancellationToken cancellationToken)
    {
        try
        {
            return await next(command, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error during executing command: {CommandType}", typeof(TCommand).FullName);
            return new Result<Unit>(new CommandHandlingError(typeof(TCommand), ex));
        }
    }
}

internal record CommandHandlingError(Type CommandType, Exception Exception) 
    : Error($"Unhandled error during executing command: {CommandType}", null)
{
    public Type CommandType { get; init; } = CommandType;
}

internal static class ServiceCollectionExtensions
{
    public static OrderedCommandHandlerPipesDescriptor UseFluentValidationPipe(
        this OrderedCommandHandlerPipesDescriptor descriptor)
    {
        descriptor.WithNext(typeof(HandlerErrorHandlingPipe<>));
        return descriptor;
    }
}