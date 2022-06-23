using CSharpMicroManager.CQRS.Abstractions.Attributes.Command;
using CSharpMicroManager.CQRS.Abstractions.Commands;

namespace CSharpMicroManager.CQRS.Extensions.Cache.Command;

/// <summary>
/// Represents cached information about instances of <see cref="ICommand"/>
/// </summary>
public interface ICommandMetadataCache
{
    /// <summary>
    /// Gets cached information about <see cref="ICommand"/>
    /// </summary>
    /// <typeparam name="TCommand">Type of the command</typeparam>
    /// <returns></returns>
    ValueTask<CachedCommandInfo> GetCommandInfo<TCommand>(CancellationToken cancellationToken = default) where TCommand : ICommand;
}

/// <summary>
/// Represents information about <see cref="ICommand"/> details
/// </summary>
public sealed class CachedCommandInfo
{
    /// <summary>
    /// Underlying type of <see cref="ICommand"/>
    /// </summary>
    public Type CommandType { get; }

    /// <summary>
    /// List of all required permissions to execute <see cref="ICommand"/>
    /// <para></para>
    /// Contains information about both: command and properties permissions
    /// </summary>
    public List<CommandPermission> RequiredPermissions { get; }

    
    /// <summary>
    /// Version of the command (1 if not specified)
    /// </summary>
    public ushort Version { get; }
    
    /// <summary>
    /// Default constructor
    /// </summary>
    /// <param name="type">Type of <see cref="ICommand"/></param>
    /// <param name="permissions">List of all required permissions to execute <see cref="ICommand"/>. Contains information about both: command and properties permissions</param>
    /// <param name="version">Version of <see cref="ICommand"/></param>
    public CachedCommandInfo(Type type, IEnumerable<CommandPermission> permissions, ushort version)
    {
        CommandType = type;
        RequiredPermissions = permissions?.ToList() ?? new List<CommandPermission>();
        Version = version;
    }
}