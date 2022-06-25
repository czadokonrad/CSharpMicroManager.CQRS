using CSharpMicroManager.CQRS.Abstractions.Commands;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Command.Handle;
using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Pipelines.Command;

internal sealed class CommandHandlerPipelineBuilder<TCommand> : ICommandHandlerPipelineBuilder<TCommand>
    where TCommand : ICommand
{

    private readonly
        List<Func<CommandHandlerPipelineDelegate<TCommand>, CommandHandlerPipelineDelegate<TCommand>>> _pipes =
            new();

    public IReadOnlyCollection<
        Func<CommandHandlerPipelineDelegate<TCommand>, CommandHandlerPipelineDelegate<TCommand>>> Pipes =>
        _pipes.ToList();

    public ICommandHandlerPipelineBuilder<TCommand> UsePipe(
        Func<CommandHandlerPipelineDelegate<TCommand>, CommandHandlerPipelineDelegate<TCommand>> middleware)
    {
        _pipes.Add(middleware);
        return this;
    }

    public ICommandHandlerPipelineBuilder<TCommand> UsePipe<TCommandHandlerPipe>()
        where TCommandHandlerPipe : class, ICommandHandlerPipe<TCommand>, new() =>
        UsePipe(new TCommandHandlerPipe());

    public ICommandHandlerPipelineBuilder<TCommand> UsePipe(ICommandHandlerPipe<TCommand> pipe)
    {
        _pipes.Add(next => { return (context, cancellationToken) => pipe.Handle(context, next, cancellationToken); });

        return this;
    }
    
    public CommandHandlerPipelineDelegate<TCommand> Build(IEnumerable<ICommandHandlerPipe<TCommand>> pipes) 
    {
        foreach (var pipe in pipes)
        {
            UsePipe(next => (command, ct) => pipe.Handle(command, next, ct));
        }

        return Build();
    }

    public CommandHandlerPipelineDelegate<TCommand> Build()
    {
        CommandHandlerPipelineDelegate<TCommand> pipeline = (_, _) =>
            Task.FromResult(new Result<Unit>(Unit.Value));

        for (var i = _pipes.Count - 1; i >= 0; i--)
        {
            pipeline = _pipes[i](pipeline);
        }

        return pipeline;
    }
}