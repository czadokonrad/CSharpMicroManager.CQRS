using CSharpMicroManager.CQRS.Abstractions.Dispatching.Query;
using CSharpMicroManager.CQRS.Abstractions.Queries;
using CSharpMicroManager.CQRS.Pipelines.Query;
using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Dispatching.Query;

public sealed class QueryDispatcher : IQueryDispatcher
{
    private readonly QueryPipelineBuilderFactory _queryPipelineBuilderFactory;

    public QueryDispatcher(QueryPipelineBuilderFactory queryPipelineBuilderFactory)
    {
        _queryPipelineBuilderFactory = queryPipelineBuilderFactory;
    }

    public Task<Result<Option<TResult>>> Handle<TQuery, TResult>(TQuery query, CancellationToken cancellationToken)
        where TQuery : IQuery<TResult> =>
        _queryPipelineBuilderFactory.CreatePipeline<TQuery, TResult>().Handle(query, cancellationToken);
}