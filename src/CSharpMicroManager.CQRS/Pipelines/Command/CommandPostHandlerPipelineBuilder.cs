using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.PostHandle;

namespace CSharpMicroManager.CQRS.Pipelines.Command;

internal sealed class CommandPostHandlerPipelineBuilder<TCommand> : ICommandPostHandlerPipelineBuilder<TCommand>
    where TCommand : ICommand
{

    private readonly List<Func<CommandPostHandlerPipelineDelegate<TCommand>, CommandPostHandlerPipelineDelegate<TCommand>>> _pipes = new();

    public IReadOnlyCollection<Func<CommandPostHandlerPipelineDelegate<TCommand>, CommandPostHandlerPipelineDelegate<TCommand>>> Pipes =>
        _pipes.ToList();

    public ICommandPostHandlerPipelineBuilder<TCommand> UsePipe(Func<CommandPostHandlerPipelineDelegate<TCommand>, CommandPostHandlerPipelineDelegate<TCommand>> middleware)
    {
        _pipes.Add(middleware);
        return this;
    }

    public ICommandPostHandlerPipelineBuilder<TCommand> UsePipe<TCommandPostHandlerPipe>() 
        where TCommandPostHandlerPipe : class, ICommandPostHandlerPipe<TCommand>, new() =>
        UsePipe(new TCommandPostHandlerPipe());

    public ICommandPostHandlerPipelineBuilder<TCommand> UsePipe(ICommandPostHandlerPipe<TCommand> pipe)
    {
        _pipes.Add(next =>
        {
            return (context, cancellationToken) => pipe.Handle(context, next, cancellationToken);
        });

        return this;
    }

    public CommandPostHandlerPipelineDelegate<TCommand> Build()
    {
        CommandPostHandlerPipelineDelegate<TCommand> pipeline = (ctx, _) => 
            Task.FromResult(ctx.CommandHandlerResult);

        for (int i = _pipes.Count - 1; i >= 0; i--)
        {
            pipeline = _pipes[i](pipeline);
        }

        return pipeline;
    }
}