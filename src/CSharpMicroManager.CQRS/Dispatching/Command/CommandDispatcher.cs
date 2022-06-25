using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Dispatching.Command;
using CSharpMicroManager.CQRS.Pipelines.Command;
using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Dispatching.Command;

internal sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly ServiceResolver _serviceResolver;

    public CommandDispatcher(ServiceResolver serviceResolver)
    {
        _serviceResolver = serviceResolver;
    }

    public Task<Result<Unit>> Handle<TCommand>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand =>
        _serviceResolver
            .Get<CommandPipelineBuilderFactory<TCommand>>()
            .CreatePipeline()
            .Handle(command, cancellationToken);
}

public delegate object ServiceResolver(Type type);

public static class ServiceResolverExtensions
{
    public static T Get<T>(this ServiceResolver serviceResolver) where T : class
    {
        var service = serviceResolver(typeof(T));
        return (T) service;
    }
}