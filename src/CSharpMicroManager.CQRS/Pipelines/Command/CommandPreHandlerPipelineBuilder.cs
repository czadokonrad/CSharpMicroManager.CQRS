using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PreHandle;
using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Pipelines.Command;

internal sealed class CommandPreHandlerPipelineBuilder<TCommand> : ICommandPreHandlerPipelineBuilder<TCommand>
    where TCommand : ICommand
{
        
    private readonly List<Func<CommandPreHandlerPipelineDelegate<TCommand>, CommandPreHandlerPipelineDelegate<TCommand>>> _pipes = new();

    public IReadOnlyCollection<Func<CommandPreHandlerPipelineDelegate<TCommand>, CommandPreHandlerPipelineDelegate<TCommand>>> Pipes =>
        _pipes.ToList();

    public ICommandPreHandlerPipelineBuilder<TCommand> UsePipe(Func<CommandPreHandlerPipelineDelegate<TCommand>, CommandPreHandlerPipelineDelegate<TCommand>> middleware)
    {
        _pipes.Add(middleware);
        return this;
    }

    public ICommandPreHandlerPipelineBuilder<TCommand> UsePipe<TCommandPreHandlerPipe>() 
        where TCommandPreHandlerPipe : class, ICommandPreHandlerPipe<TCommand>, new() =>
        UsePipe(new TCommandPreHandlerPipe());

    public ICommandPreHandlerPipelineBuilder<TCommand> UsePipe(ICommandPreHandlerPipe<TCommand> pipe)
    {
        _pipes.Add(next =>
        {
            return (context, cancellationToken) => pipe.Handle(context, next, cancellationToken);
        });

        return this;
    }

    public CommandPreHandlerPipelineDelegate<TCommand> Build()
    {
        CommandPreHandlerPipelineDelegate<TCommand> pipeline = (_, _) => 
            Task.FromResult(new Result<Unit>(Unit.Value));

        for (int i = _pipes.Count - 1; i >= 0; i--)
        {
            pipeline = _pipes[i](pipeline);
        }

        return pipeline;
    }
}