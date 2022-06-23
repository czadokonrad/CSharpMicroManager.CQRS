using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PreHandle;
using CSharpMicroManager.CQRS.Extensions.Cache.Command;
using CSharpMicroManager.CQRS.Pipelines.Command.Descriptors;
using CSharpMicroManager.Functional.Core;
using Microsoft.Extensions.DependencyInjection;

namespace CSharpMicroManager.CQRS.Extensions.Authorization.Command.PreHandle;

/// <summary>
/// Verifies that, the use that made request with <see cref="TCommand"/> has permission to execute it if command has attribute
/// <see cref="RequirePermission"/>
/// and has access to all the fields in <see cref="TCommand"/> that can be protected also by <see cref="RequirePermission"/>
/// </summary>
/// <typeparam name="TCommand"></typeparam>
internal sealed class CanExecuteRequestedCommandPipe<TCommand> : ICommandPreHandlerPipe<TCommand>
    where TCommand : ICommand
{
    private readonly ICommandMetadataCache _commandMetadataCache;
    private readonly IUserContext _userContext;

    public CanExecuteRequestedCommandPipe(
        ICommandMetadataCache commandMetadataCache,
        IUserContext userContext)
    {
        _commandMetadataCache = commandMetadataCache;
        _userContext = userContext;
    }
    
    public async Task<Result<Unit>> Handle(
        CommandPreHandlerPipelineContext<TCommand> context, 
        CommandPreHandlerPipelineDelegate<TCommand> next,
        CancellationToken cancellationToken)
    {
        if (await CanExecuteCommand())
        {
            return await next(context, cancellationToken);
        }

        return new Result<Unit>(new CannotExecuteRequestedCommandError());
    }
    
    private async ValueTask<bool> CanExecuteCommand()
    {
        var commandInfo = await _commandMetadataCache.GetCommandInfo<TCommand>();
        return commandInfo
            .RequiredPermissions
            .All(rp => _userContext.GetAllUserPermissionIds().Contains(rp.Id));
    }
}

//TODO: Change to BusinessError
internal sealed record CannotExecuteRequestedCommandError() : Error(
    "Cannot execute requested command, because permissions are not sufficient", null);

public static class ServiceCollectionExtensions
{
    public static OrderedCommandPreHandlerPipesDescriptor UseCanExecuteRequestedCommandPipe(this OrderedCommandPreHandlerPipesDescriptor descriptor)
    {
        RegisterRequiredServices(descriptor);
        return descriptor.WithNext(typeof(CanExecuteRequestedCommandPipe<>));
    }

    private static void RegisterRequiredServices(OrderedCommandPipeHandlerPipesDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor.PipelinesDescriptor.Services);
        descriptor.PipelinesDescriptor.Services.AddSingleton<ICommandMetadataCache, LocalCommandMetadataCache>();
    }
}

//TODO in other package
public interface IUserContext
{
    IReadOnlyCollection<Guid> GetAllUserPermissionIds();
}

public class FakeUserContext : IUserContext
{
    public IReadOnlyCollection<Guid> GetAllUserPermissionIds()
    {
        return new List<Guid>();
    }
}