using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.Handle;
using CSharpMicroManager.CQRS.Abstractions.Queries;
using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Pipelines.Query;

/// <summary>
/// Pipe which is responsible for handling actual <see cref="TQuery"/> by using <see cref="IQueryHandler{TQuery, TResult}"/>
/// </summary>
internal sealed class QueryHandlingPipe<TQuery, TResult> : IQueryHandlerPipe<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    private readonly IQueryHandler<TQuery, TResult> _queryHandler;

    public QueryHandlingPipe(IQueryHandler<TQuery, TResult> queryHandler)
    {
        _queryHandler = queryHandler;
    }
    public async Task<Result<Option<TResult>>> Handle(
        TQuery query,
        QueryHandlerPipelineDelegate<TQuery, TResult> next,
        CancellationToken cancellationToken)
    {
        var queryResult = await _queryHandler.Handle(query, cancellationToken);

        if (!queryResult.IsError)
        {
            return await next(query, cancellationToken);
        }

        return queryResult;
    }
}