using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.Handle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.PostHandle;
using CSharpMicroManager.CQRS.Abstractions.Pipelines.Query.PreHandle;
using CSharpMicroManager.CQRS.Abstractions.Queries;
using CSharpMicroManager.Functional.Core;

namespace CSharpMicroManager.CQRS.Pipelines.Query;

public class QueryHandlerPipelineWrapper<TQuery, TResult> where TQuery : IQuery<TResult>
{
    private readonly QueryPreHandlerPipelineDelegate<TQuery, TResult> _preHandle;
    private readonly QueryHandlerPipelineDelegate<TQuery, TResult> _next;
    private readonly QueryPostHandlerPipelineDelegate<TQuery, TResult> _postHandle;

    public QueryHandlerPipelineWrapper(
        QueryPreHandlerPipelineDelegate<TQuery, TResult> preHandle,
        QueryHandlerPipelineDelegate<TQuery, TResult> next,
        QueryPostHandlerPipelineDelegate<TQuery, TResult> postHandle)
    {
        _preHandle = preHandle;
        _next = next;
        _postHandle = postHandle;
    }
    
    public async Task<Result<Option<TResult>>> Handle(TQuery query, CancellationToken cancellationToken)
    {
        var result = await _preHandle(query, cancellationToken);

        if (result.Errors.Any())
        {
            return result;
        }
        
        await _next(query, cancellationToken);

        return await _postHandle(query, cancellationToken);
    }
}