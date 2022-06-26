using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.Handle;
using CSharpMicroManager.CQRS.Abstractions.Queries;
using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Pipelines.Query;

internal sealed class QueryHandlerPipelineBuilder<TQuery, TResult> : IQueryHandlerPipelineBuilder<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    public IQueryHandlerPipelineBuilder<TQuery, TResult> UsePipe(
        Func<QueryHandlerPipelineDelegate<TQuery, TResult>, QueryHandlerPipelineDelegate<TQuery, TResult>>
            middleware)
    {
        _pipes.Add(middleware);
        return this;
    }

    public IQueryHandlerPipelineBuilder<TQuery, TResult> UsePipe(IQueryHandlerPipe<TQuery, TResult> pipe) 
    {
        _pipes.Add(next =>
        {
            return (query, cancellationToken) => pipe.Handle(query, next, cancellationToken);
        });

        return this;
    }

    public IQueryHandlerPipelineBuilder<TQuery, TResult> UsePipe<TCommandHandlerPipe>()
        where TCommandHandlerPipe : class, IQueryHandlerPipe<TQuery, TResult>, new()
    {
        throw new NotImplementedException();
    }
    
    public QueryHandlerPipelineDelegate<TQuery, TResult> Build(IEnumerable<IQueryHandlerPipe<TQuery, TResult>> pipes) 
    {
        foreach (var pipe in pipes)
        {
            UsePipe(next => (query, ct) => pipe.Handle(query, next, ct));
        }

        return Build();
    }

    public QueryHandlerPipelineDelegate<TQuery, TResult> Build()
    {
        QueryHandlerPipelineDelegate<TQuery, TResult> pipeline = (_, _) => 
            Task.FromResult(new Result<Option<TResult>>(F.None));

        for (var i = _pipes.Count - 1; i >= 0; i--)
        {
            pipeline = _pipes[i](pipeline);
        }
        
        return pipeline;
    }
    
    private readonly List<Func<QueryHandlerPipelineDelegate<TQuery, TResult>, QueryHandlerPipelineDelegate<TQuery, TResult>>> _pipes = new();

    public IReadOnlyCollection<Func<QueryHandlerPipelineDelegate<TQuery, TResult>,
        QueryHandlerPipelineDelegate<TQuery, TResult>>> Pipes => _pipes.ToList();
}