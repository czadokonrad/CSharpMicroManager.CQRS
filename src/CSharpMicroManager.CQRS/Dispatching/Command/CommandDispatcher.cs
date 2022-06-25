using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Dispatching.Command;
using CSharpMicroManager.CQRS.Pipelines.Command;
using CSharpMicroManager.Functional.Core;
using Microsoft.Extensions.DependencyInjection;

namespace CSharpMicroManager.CQRS.Dispatching.Command;

internal sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<Result<Unit>> Handle<TCommand>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand =>
        _serviceProvider
            .GetRequiredService<CommandPipelineBuilderFactory<TCommand>>()
            .CreatePipeline()
            .Handle(command, cancellationToken);
}