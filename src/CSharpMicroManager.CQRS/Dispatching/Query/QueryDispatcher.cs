using CSharpMicroManager.CQRS.Abstractions.Dispatching.Query;
using CSharpMicroManager.CQRS.Abstractions.Queries;
using CSharpMicroManager.CQRS.Dispatching.Command;
using CSharpMicroManager.CQRS.Pipelines.Query;
using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Dispatching.Query;

public sealed class QueryDispatcher : IQueryDispatcher
{
    private readonly ServiceResolver _serviceResolver;

    public QueryDispatcher(ServiceResolver serviceResolver)
    {
        _serviceResolver = serviceResolver;
    }

    public Task<Result<Option<TResult>>> Handle<TQuery, TResult>(TQuery query, CancellationToken cancellationToken)
        where TQuery : IQuery<TResult> =>
        _serviceResolver
            .Get<QueryPipelineBuilderFactory<TQuery, TResult>>()
            .CreatePipeline()
            .Handle(query, cancellationToken);
}