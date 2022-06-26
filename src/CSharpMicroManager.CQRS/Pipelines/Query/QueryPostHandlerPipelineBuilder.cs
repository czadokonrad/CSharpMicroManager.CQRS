using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.PostHandle;
using CSharpMicroManager.CQRS.Abstractions.Queries;
using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Pipelines.Query;

internal sealed class QueryPostHandlerPipelineBuilder<TQuery, TResult> : IQueryPostHandlerPipelineBuilder<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    public IQueryPostHandlerPipelineBuilder<TQuery, TResult> UsePipe(
        Func<QueryPostHandlerPipelineDelegate<TQuery, TResult>, QueryPostHandlerPipelineDelegate<TQuery, TResult>>
            middleware)
    {
        _pipes.Add(middleware);
        return this;
    }

    public IQueryPostHandlerPipelineBuilder<TQuery, TResult> UsePipe(IQueryPostHandlerPipe<TQuery, TResult> pipe) 
    {
        _pipes.Add(next =>
        {
            return (query, cancellationToken) => pipe.Handle(query, next, cancellationToken);
        });

        return this;
    }

    public IQueryPostHandlerPipelineBuilder<TQuery, TResult> UsePipe<TCommandPostHandlerPipe>()
        where TCommandPostHandlerPipe : class, IQueryPostHandlerPipe<TQuery, TResult>, new()
    {
        throw new NotImplementedException();
    }

    public QueryPostHandlerPipelineDelegate<TQuery, TResult> Build(IEnumerable<IQueryPostHandlerPipe<TQuery, TResult>> pipes) 
    {
        foreach (var pipe in pipes)
        {
            UsePipe(next => (query, ct) => pipe.Handle(query, next, ct));
        }

        return Build();
    }
    
    public QueryPostHandlerPipelineDelegate<TQuery, TResult> Build()
    {
        QueryPostHandlerPipelineDelegate<TQuery, TResult> pipeline = (_, _) => 
            Task.FromResult(new Result<Option<TResult>>(F.None));

        for (int i = _pipes.Count - 1; i >= 0; i--)
        {
            pipeline = _pipes[i](pipeline);
        }

        return pipeline;
    }
    
    private readonly List<Func<QueryPostHandlerPipelineDelegate<TQuery, TResult>, QueryPostHandlerPipelineDelegate<TQuery, TResult>>> _pipes = new();

    public IReadOnlyCollection<Func<QueryPostHandlerPipelineDelegate<TQuery, TResult>,
        QueryPostHandlerPipelineDelegate<TQuery, TResult>>> Pipes => _pipes.ToList();
}