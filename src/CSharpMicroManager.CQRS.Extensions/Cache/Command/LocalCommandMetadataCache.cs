using System.Collections.Concurrent;
using System.Reflection;
using CSharpMicroManager.CQRS.Abstractions.Attributes.Command;
using CSharpMicroManager.CQRS.Abstractions.Commands;

namespace CSharpMicroManager.CQRS.Extensions.Cache.Command;

internal sealed class LocalCommandMetadataCache: ICommandMetadataCache
{
    private readonly ConcurrentDictionary<Type, CachedCommandInfo> _cache = new();
    
    public ValueTask<CachedCommandInfo> GetCommandInfo<TCommand>(CancellationToken cancellationToken) where TCommand : ICommand =>
        new(_cache.GetOrAdd(typeof(TCommand), k => CreateCommandInfo<TCommand>()));

    private CachedCommandInfo CreateCommandInfo<TCommand>() where TCommand : ICommand
    {
        var commandPermissions = typeof(TCommand)
            .GetCustomAttributesData()
            .Where(x => x.AttributeType.IsGenericType && x.AttributeType.GetGenericTypeDefinition() == typeof(RequirePermission<>))
            .Select(x => x.AttributeType.GetGenericArguments().Single())
            .Select(commandPermissionType => (CommandPermission)Activator.CreateInstance(commandPermissionType)!)
            .ToList();

        var propertiesPermissions = typeof(TCommand)
            .GetProperties()
            .SelectMany(prop => prop.GetCustomAttributesData()
                .Where(x => x.AttributeType.IsGenericType && x.AttributeType.GetGenericTypeDefinition() == typeof(RequirePermission<>))
                .Select(x => x.AttributeType.GetGenericArguments().Single()))
                .Select(commandPermissionType => (CommandPermission)Activator.CreateInstance(commandPermissionType)!)
                .ToList();

        var version = typeof(TCommand)
            .GetCustomAttributes()
            .Where(a => a.GetType() == typeof(CommandVersionAttribute))
            .Cast<CommandVersionAttribute>()
            .SingleOrDefault()
            ?.Version ?? 1;

        var permissions = commandPermissions.Concat(propertiesPermissions);
        
        return new CachedCommandInfo(typeof(TCommand), permissions, version);
    }
}