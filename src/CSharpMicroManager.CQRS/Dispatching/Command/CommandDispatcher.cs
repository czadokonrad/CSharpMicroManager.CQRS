using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Dispatching.Command;
using CSharpMicroManager.CQRS.Pipelines.Command;
using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Dispatching.Command;

internal sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly CommandPipelineBuilderFactory _pipelineBuilderFactory;

    public CommandDispatcher(CommandPipelineBuilderFactory pipelineBuilderFactory)
    {
        _pipelineBuilderFactory = pipelineBuilderFactory;
    }

    public Task<Result<Unit>> Handle<TCommand>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand =>
        _pipelineBuilderFactory.CreatePipeline<TCommand>().Handle(command, cancellationToken);
}