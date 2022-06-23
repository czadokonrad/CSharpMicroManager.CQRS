using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.PreHandle;
using CSharpMicroManager.CQRS.Abstractions.Queries;
using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Pipelines.Query;

internal sealed class QueryPreHandlerPipelineBuilder<TQuery, TResult> : IQueryPreHandlerPipelineBuilder<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    public IQueryPreHandlerPipelineBuilder<TQuery, TResult> UsePipe(
        Func<QueryPreHandlerPipelineDelegate<TQuery, TResult>, QueryPreHandlerPipelineDelegate<TQuery, TResult>>
            middleware)
    {
        _pipes.Add(middleware);
        return this;
    }

    public IQueryPreHandlerPipelineBuilder<TQuery, TResult> UsePipe(IQueryPreHandlerPipe<TQuery, TResult> pipe) 
    {
        _pipes.Add(next =>
        {
            return (query, cancellationToken) => pipe.Handle(query, next, cancellationToken);
        });

        return this;
    }

    public IQueryPreHandlerPipelineBuilder<TQuery, TResult> UsePipe<TCommandPreHandlerPipe>()
        where TCommandPreHandlerPipe : class, IQueryPreHandlerPipe<TQuery, TResult>, new()
    {
        throw new NotImplementedException();
    }

    public QueryPreHandlerPipelineDelegate<TQuery, TResult> Build()
    {
        QueryPreHandlerPipelineDelegate<TQuery, TResult> pipeline = (_, _) => 
            Task.FromResult(new Result<Option<TResult>>(F.None));

        for (var i = _pipes.Count - 1; i >= 0; i--)
        {
            pipeline = _pipes[i](pipeline);
        }

        return pipeline;
    }
    
    private readonly List<Func<QueryPreHandlerPipelineDelegate<TQuery, TResult>, QueryPreHandlerPipelineDelegate<TQuery, TResult>>> _pipes = new();

    public IReadOnlyCollection<Func<QueryPreHandlerPipelineDelegate<TQuery, TResult>,
        QueryPreHandlerPipelineDelegate<TQuery, TResult>>> Pipes => _pipes.ToList();
}